import { Cube, DotsThree, House, SignOut } from "@phosphor-icons/react";
import {
	createFileRoute,
	Link,
	Outlet,
	useNavigate,
	useRouterState,
} from "@tanstack/react-router";

import { useAuth } from "#/auth/AuthProvider";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import {
	Sidebar,
	SidebarContent,
	SidebarFooter,
	SidebarGroup,
	SidebarGroupContent,
	SidebarGroupLabel,
	SidebarHeader,
	SidebarInset,
	SidebarMenu,
	SidebarMenuButton,
	SidebarMenuItem,
	SidebarProvider,
	SidebarRail,
	SidebarTrigger,
} from "@/components/ui/sidebar";

export const Route = createFileRoute("/_app")({
	component: AppLayout,
});

function initials(name: string) {
	return name
		.split(/\s+/)
		.filter(Boolean)
		.slice(0, 2)
		.map((part) => part[0]?.toUpperCase() ?? "")
		.join("");
}

function AppLayout() {
	const { session, logout } = useAuth();
	const navigate = useNavigate();
	const pathname = useRouterState({ select: (s) => s.location.pathname });

	return (
		<SidebarProvider>
			<Sidebar collapsible="icon">
				<SidebarHeader>
					<SidebarMenu>
						<SidebarMenuItem>
							<SidebarMenuButton size="lg" asChild>
								<Link to="/">
									<div className="flex aspect-square size-8 items-center justify-center rounded-lg bg-primary text-primary-foreground">
										<Cube className="size-4" weight="bold" />
									</div>
									<div className="flex flex-col gap-0.5 leading-none">
										<span className="font-medium">Pablo</span>
										<span className="text-xs text-muted-foreground">
											Workspace
										</span>
									</div>
								</Link>
							</SidebarMenuButton>
						</SidebarMenuItem>
					</SidebarMenu>
				</SidebarHeader>

				<SidebarContent>
					<SidebarGroup>
						<SidebarGroupLabel>General</SidebarGroupLabel>
						<SidebarGroupContent>
							<SidebarMenu>
								<SidebarMenuItem>
									<SidebarMenuButton asChild isActive={pathname === "/"}>
										<Link to="/">
											<House />
											<span>Home</span>
										</Link>
									</SidebarMenuButton>
								</SidebarMenuItem>
							</SidebarMenu>
						</SidebarGroupContent>
					</SidebarGroup>
				</SidebarContent>

				<SidebarFooter>
					<SidebarMenu>
						<SidebarMenuItem>
							{session ? (
								<div className="flex items-center gap-2 px-2 py-1.5">
									<Avatar size="sm">
										<AvatarFallback>
											{initials(session.displayName)}
										</AvatarFallback>
									</Avatar>
									<div className="flex min-w-0 flex-1 flex-col group-data-[collapsible=icon]:hidden">
										<span className="truncate text-sm font-medium">
											{session.displayName}
										</span>
										<span className="truncate text-xs text-muted-foreground">
											{session.email}
										</span>
									</div>
									<DropdownMenu>
										<DropdownMenuTrigger asChild>
											<Button
												variant="ghost"
												size="icon-sm"
												aria-label="Account"
												className="group-data-[collapsible=icon]:hidden"
											>
												<DotsThree />
											</Button>
										</DropdownMenuTrigger>
										<DropdownMenuContent align="end" side="top">
											<DropdownMenuItem onClick={() => logout()}>
												<SignOut />
												Log out
											</DropdownMenuItem>
										</DropdownMenuContent>
									</DropdownMenu>
								</div>
							) : (
								<SidebarMenuButton
									onClick={() => {
										void navigate({ to: "/login" });
									}}
								>
									<span>Log in</span>
								</SidebarMenuButton>
							)}
						</SidebarMenuItem>
					</SidebarMenu>
				</SidebarFooter>
				<SidebarRail />
			</Sidebar>

			<SidebarInset>
				<header className="flex h-12 items-center gap-2 border-b px-4">
					<SidebarTrigger />
				</header>
				<div className="flex flex-1 flex-col p-6">
					<Outlet />
				</div>
			</SidebarInset>
		</SidebarProvider>
	);
}
