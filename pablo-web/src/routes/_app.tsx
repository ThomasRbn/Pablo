import { DotsThree, House, SignOut } from "@phosphor-icons/react";
import {
	createFileRoute,
	Link,
	Outlet,
	useNavigate,
	useRouterState,
} from "@tanstack/react-router";

import { useAuth } from "#/auth/AuthProvider";
import { ThemeToggle } from "@/components/theme-toggle";
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
					<ThemeToggle />
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
					<div className="flex items-center gap-1 group-data-[collapsible=icon]:flex-col-reverse group-data-[collapsible=icon]:items-center group-data-[collapsible=icon]:gap-2">
						<SidebarTrigger size="icon" />
						{session ? (
							<div className="flex min-w-0 flex-1 items-center gap-2 px-2 py-1.5 group-data-[collapsible=icon]:flex-none group-data-[collapsible=icon]:justify-center group-data-[collapsible=icon]:p-0">
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
										{session.username}
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
								className="group-data-[collapsible=icon]:hidden"
								onClick={() => {
									void navigate({ to: "/login" });
								}}
							>
								<span>Log in</span>
							</SidebarMenuButton>
						)}
					</div>
				</SidebarFooter>
				<SidebarRail />
			</Sidebar>

			<SidebarInset>
				<div className="flex flex-1 flex-col p-6">
					<Outlet />
				</div>
			</SidebarInset>
		</SidebarProvider>
	);
}
