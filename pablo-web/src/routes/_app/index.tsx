import { createFileRoute } from "@tanstack/react-router";

import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
	Card,
	CardContent,
	CardDescription,
	CardHeader,
	CardTitle,
} from "@/components/ui/card";

export const Route = createFileRoute("/_app/")({ component: Home });

function Home() {
	return (
		<div className="mx-auto flex w-full max-w-3xl flex-col gap-6">
			<div className="flex flex-col gap-2">
				<h1 className="text-3xl font-semibold tracking-tight">Pablo</h1>
				<p className="text-lg text-muted-foreground">
					A sample surface wired to your shadcn theme — primary brand orange,
					typography, and semantic colors included.
				</p>
			</div>

			<div className="flex flex-wrap items-center gap-3">
				<Button>Get started</Button>
				<Button variant="secondary">View tokens</Button>
				<Button variant="ghost">Docs</Button>
			</div>

			<Alert>
				<AlertTitle>Theme connected</AlertTitle>
				<AlertDescription>
					Components below inherit Pablo tokens via CSS variables in the global
					stylesheet. Primary is #FF6900.
				</AlertDescription>
			</Alert>

			<section className="flex flex-col gap-3">
				<h2 className="text-xl font-semibold">Actions</h2>
				<div className="flex flex-wrap items-center gap-3">
					<Button>Primary</Button>
					<Button variant="secondary">Secondary</Button>
					<Button variant="ghost">Ghost</Button>
					<Button variant="destructive">Destructive</Button>
				</div>
			</section>

			<section className="flex flex-col gap-3">
				<h2 className="text-xl font-semibold">Status</h2>
				<div className="flex flex-wrap items-center gap-2">
					<Badge className="bg-emerald-600 text-white hover:bg-emerald-600">
						Active
					</Badge>
					<Badge className="bg-amber-400 text-foreground hover:bg-amber-400">
						Pending
					</Badge>
					<Badge variant="destructive">Failed</Badge>
					<Badge variant="secondary">In review</Badge>
					<Badge variant="outline">Draft</Badge>
				</div>
			</section>

			<section className="flex flex-col gap-3">
				<h2 className="text-xl font-semibold">Surfaces</h2>
				<div className="grid gap-3 sm:grid-cols-2">
					<Card>
						<CardHeader>
							<CardTitle>Accent card</CardTitle>
							<CardDescription>
								Card surfaces, borders, and radius come from your theme tokens.
							</CardDescription>
						</CardHeader>
						<CardContent>
							<p className="text-sm text-primary">Primary text token</p>
						</CardContent>
					</Card>
					<Card>
						<CardHeader>
							<CardTitle>Muted card</CardTitle>
							<CardDescription>
								Muted backgrounds help group secondary content without extra
								chrome.
							</CardDescription>
						</CardHeader>
						<CardContent>
							<Button variant="secondary" size="sm">
								Open
							</Button>
						</CardContent>
					</Card>
				</div>
			</section>
		</div>
	);
}
