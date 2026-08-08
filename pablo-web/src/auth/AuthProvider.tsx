import {
	createContext,
	type ReactNode,
	useCallback,
	useContext,
	useMemo,
	useState,
} from "react";

import { postApiAuthLogin } from "#/api/generated/auth/auth";
import type { LoginRequest, LoginResponse } from "#/api/generated/models";
import { clearSession, readSession, writeSession } from "#/auth/storage";

type AuthContextValue = {
	session: LoginResponse | null;
	accessToken: string | null;
	isAuthenticated: boolean;
	login: (body: LoginRequest) => Promise<LoginResponse>;
	logout: () => void;
};

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
	const [session, setSession] = useState<LoginResponse | null>(() =>
		readSession(),
	);

	const login = useCallback(async (body: LoginRequest) => {
		const response = await postApiAuthLogin(body);
		writeSession(response);
		setSession(response);
		return response;
	}, []);

	const logout = useCallback(() => {
		clearSession();
		setSession(null);
	}, []);

	const value = useMemo<AuthContextValue>(
		() => ({
			session,
			accessToken: session?.accessToken ?? null,
			isAuthenticated: session !== null,
			login,
			logout,
		}),
		[session, login, logout],
	);

	return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
	const context = useContext(AuthContext);
	if (!context) {
		throw new Error("useAuth must be used within an AuthProvider");
	}
	return context;
}
