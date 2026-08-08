import { Cube } from "@phosphor-icons/react";
import { createFileRoute, redirect, useNavigate } from "@tanstack/react-router";
import { type FormEvent, useState } from "react";

import { ApiError } from "#/api/mutator";
import { useAuth } from "#/auth/AuthProvider";
import { readSession } from "#/auth/storage";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import {
	Card,
	CardContent,
	CardDescription,
	CardHeader,
	CardTitle,
} from "@/components/ui/card";
import { Field, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Spinner } from "@/components/ui/spinner";

export const Route = createFileRoute("/login")({
	beforeLoad: () => {
		if (readSession()) {
			throw redirect({ to: "/" });
		}
	},
	component: LoginPage,
});

function LoginPage() {
	const { login } = useAuth();
	const navigate = useNavigate();
	const [username, setUsername] = useState("");
	const [password, setPassword] = useState("");
	const [isLoading, setIsLoading] = useState(false);
	const [error, setError] = useState("");

	const handleLogIn = async (event: FormEvent<HTMLFormElement>) => {
		event.preventDefault();
		setError("");
		if (!username || !password) {
			setError("Please enter both username and password.");
			return;
		}

		setIsLoading(true);
		try {
			await login({ username, password });
			await navigate({ to: "/" });
		} catch (err) {
			if (err instanceof ApiError) {
				setError(err.message);
			} else if (err instanceof Error) {
				setError(err.message);
			} else {
				setError("Unable to log in. Please try again.");
			}
		} finally {
			setIsLoading(false);
		}
	};

	return (
		<div className="flex min-h-svh items-center justify-center bg-muted/40 p-6">
			<div className="flex w-full max-w-sm flex-col items-center gap-6">
				<div className="flex flex-col items-center gap-2">
					<Cube className="size-8 text-primary" weight="duotone" />
					<p className="text-base font-semibold">Pablo</p>
				</div>

				<Card className="w-full">
					<CardHeader className="text-center">
						<CardTitle>Log in</CardTitle>
						<CardDescription>
							Enter your credentials to continue
						</CardDescription>
					</CardHeader>
					<CardContent>
						<form onSubmit={handleLogIn}>
							<FieldGroup>
								{error ? (
									<Alert variant="destructive">
										<AlertTitle>Log in failed</AlertTitle>
										<AlertDescription>{error}</AlertDescription>
									</Alert>
								) : null}

								<Field>
									<FieldLabel htmlFor="username">Username or email</FieldLabel>
									<Input
										id="username"
										type="text"
										autoComplete="username"
										placeholder="root or you@example.com"
										value={username}
										onChange={(e) => setUsername(e.target.value)}
									/>
								</Field>

								<Field>
									<FieldLabel htmlFor="password">Password</FieldLabel>
									<Input
										id="password"
										type="password"
										autoComplete="current-password"
										placeholder="Enter your password"
										value={password}
										onChange={(e) => setPassword(e.target.value)}
									/>
								</Field>

								<Button
									type="submit"
									size="lg"
									className="w-full"
									disabled={isLoading}
								>
									{isLoading ? <Spinner data-icon="inline-start" /> : null}
									Log in
								</Button>
							</FieldGroup>
						</form>
					</CardContent>
				</Card>
			</div>
		</div>
	);
}
