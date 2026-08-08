import type { LoginResponse } from "#/api/generated/models";

export const AUTH_STORAGE_KEY = "pablo.auth";

type StoredSession = LoginResponse & {
	expiresAt?: number;
};

function getJwtExpiryMs(accessToken: string): number | null {
	try {
		const payload = accessToken.split(".")[1];
		if (!payload) {
			return null;
		}
		const normalized = payload.replace(/-/g, "+").replace(/_/g, "/");
		const padded = normalized.padEnd(
			normalized.length + ((4 - (normalized.length % 4)) % 4),
			"=",
		);
		const json = JSON.parse(atob(padded)) as { exp?: unknown };
		return typeof json.exp === "number" ? json.exp * 1000 : null;
	} catch {
		return null;
	}
}

function resolveExpiresAt(session: StoredSession): number | null {
	if (typeof session.expiresAt === "number") {
		return session.expiresAt;
	}

	return getJwtExpiryMs(session.accessToken);
}

export function readSession(): LoginResponse | null {
	if (typeof localStorage === "undefined") {
		return null;
	}

	try {
		const raw = localStorage.getItem(AUTH_STORAGE_KEY);
		if (!raw) {
			return null;
		}
		const stored = JSON.parse(raw) as StoredSession;
		const expiresAt = resolveExpiresAt(stored);
		if (expiresAt !== null && Date.now() >= expiresAt) {
			clearSession();
			return null;
		}
		return stored;
	} catch {
		return null;
	}
}

export function writeSession(session: LoginResponse): void {
	if (typeof localStorage === "undefined") {
		return;
	}

	const expiresInSeconds = Number(session.expiresIn);
	const expiresAt = Number.isFinite(expiresInSeconds)
		? Date.now() + expiresInSeconds * 1000
		: (getJwtExpiryMs(session.accessToken) ?? undefined);

	const stored: StoredSession = { ...session, expiresAt };
	localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(stored));
}

export function clearSession(): void {
	if (typeof localStorage === "undefined") {
		return;
	}

	localStorage.removeItem(AUTH_STORAGE_KEY);
}

export function getAccessToken(): string | null {
	return readSession()?.accessToken ?? null;
}
