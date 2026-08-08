import type { LoginResponse } from "#/api/generated/models";

export const AUTH_STORAGE_KEY = "pablo.auth";

export function readSession(): LoginResponse | null {
	if (typeof localStorage === "undefined") {
		return null;
	}

	try {
		const raw = localStorage.getItem(AUTH_STORAGE_KEY);
		if (!raw) {
			return null;
		}
		return JSON.parse(raw) as LoginResponse;
	} catch {
		return null;
	}
}

export function writeSession(session: LoginResponse): void {
	localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(session));
}

export function clearSession(): void {
	localStorage.removeItem(AUTH_STORAGE_KEY);
}

export function getAccessToken(): string | null {
	return readSession()?.accessToken ?? null;
}
