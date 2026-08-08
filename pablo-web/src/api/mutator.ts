import { getAccessToken } from "#/auth/storage";

export type ProblemDetails = {
	type?: string;
	title?: string;
	status?: number;
	detail?: string;
	instance?: string;
	[key: string]: unknown;
};

export class ApiError extends Error {
	readonly status: number;
	readonly problem: ProblemDetails | null;

	constructor(status: number, problem: ProblemDetails | null, message: string) {
		super(message);
		this.name = "ApiError";
		this.status = status;
		this.problem = problem;
	}
}

export const customFetch = async <T>(
	url: string,
	options: RequestInit,
): Promise<T> => {
	const headers = new Headers(options.headers);
	if (!headers.has("Content-Type") && options.body) {
		headers.set("Content-Type", "application/json");
	}

	const token = getAccessToken();
	if (token && !headers.has("Authorization")) {
		headers.set("Authorization", `Bearer ${token}`);
	}

	const response = await fetch(url, {
		...options,
		headers,
	});

	if (response.status === 204) {
		return undefined as T;
	}

	const text = await response.text();
	let data: unknown = null;
	if (text) {
		try {
			data = JSON.parse(text) as unknown;
		} catch {
			data = text;
		}
	}

	if (!response.ok) {
		const problem =
			data && typeof data === "object" ? (data as ProblemDetails) : null;
		const message =
			problem?.detail ??
			problem?.title ??
			`Request failed with status ${response.status}`;
		throw new ApiError(response.status, problem, message);
	}

	return data as T;
};
