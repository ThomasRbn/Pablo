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
	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");
	const [isLoading, setIsLoading] = useState(false);
	const [error, setError] = useState("");

	const handleSignIn = async (event: FormEvent<HTMLFormElement>) => {
		event.preventDefault();
		setError("");
		if (!email || !password) {
			setError("Please enter both email and password.");
			return;
		}

		setIsLoading(true);
		try {
			await login({ email, password });
			await navigate({ to: "/" });
		} catch (err) {
			if (err instanceof ApiError) {
				setError(err.message);
			} else if (err instanceof Error) {
				setError(err.message);
			} else {
				setError("Unable to sign in. Please try again.");
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
						<CardTitle>Sign in</CardTitle>
						<CardDescription>
							Enter your credentials to continue
						</CardDescription>
					</CardHeader>
					<CardContent>
						<form onSubmit={handleSignIn}>
							<FieldGroup>
								{error ? (
									<Alert variant="destructive">
										<AlertTitle>Sign in failed</AlertTitle>
										<AlertDescription>{error}</AlertDescription>
									</Alert>
								) : null}

								<Field>
									<FieldLabel htmlFor="email">Email</FieldLabel>
									<Input
										id="email"
										type="email"
										autoComplete="email"
										placeholder="you@example.com"
										value={email}
										onChange={(e) => setEmail(e.target.value)}
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
									Sign in
								</Button>
							</FieldGroup>
						</form>
					</CardContent>
				</Card>
			</div>
		</div>
	);
}
