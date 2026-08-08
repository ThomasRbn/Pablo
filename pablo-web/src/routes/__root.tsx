import { createRootRoute, Outlet } from "@tanstack/react-router";
import { lazy, Suspense } from "react";

import { AuthProvider } from "#/auth/AuthProvider";

const Devtools = import.meta.env.DEV
	? lazy(() =>
			Promise.all([
				import("@tanstack/react-devtools"),
				import("@tanstack/react-router-devtools"),
			]).then(([{ TanStackDevtools }, { TanStackRouterDevtoolsPanel }]) => ({
				default: () => (
					<TanStackDevtools
						config={{
							position: "bottom-right",
						}}
						plugins={[
							{
								name: "TanStack Router",
								render: <TanStackRouterDevtoolsPanel />,
							},
						]}
					/>
				),
			})),
		)
	: null;

export const Route = createRootRoute({
	component: RootComponent,
});

function RootComponent() {
	return (
		<AuthProvider>
			<Outlet />
			{Devtools ? (
				<Suspense>
					<Devtools />
				</Suspense>
			) : null}
		</AuthProvider>
	);
}
