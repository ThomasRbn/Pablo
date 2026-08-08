import { clearSession, getAccessToken } from "#/auth/storage";

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

function resolveUrl(url: string): string {
	const baseUrl = import.meta.env.VITE_API_BASE_URL as string | undefined;
	if (!baseUrl) {
		return url;
	}
	return `${baseUrl.replace(/\/$/, "")}${url.startsWith("/") ? url : `/${url}`}`;
}

function redirectToLoginOnUnauthorized(): void {
	if (typeof window === "undefined") {
		return;
	}
	if (window.location.pathname === "/login") {
		return;
	}
	window.location.assign("/login");
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

	const response = await fetch(resolveUrl(url), {
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
		// Only clear on 401 when we sent a bearer token — failed logins are also 401.
		if (response.status === 401 && token) {
			clearSession();
			redirectToLoginOnUnauthorized();
		}

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
