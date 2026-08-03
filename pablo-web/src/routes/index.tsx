import { AppShell } from "@astryxdesign/core/AppShell";
import { Badge } from "@astryxdesign/core/Badge";
import { Banner } from "@astryxdesign/core/Banner";
import { Button } from "@astryxdesign/core/Button";
import { Card } from "@astryxdesign/core/Card";
import { Center } from "@astryxdesign/core/Center";
import { Grid } from "@astryxdesign/core/Grid";
import { Stack } from "@astryxdesign/core/Layout";
import { Section } from "@astryxdesign/core/Section";
import { StatusDot } from "@astryxdesign/core/StatusDot";
import { Heading, Text } from "@astryxdesign/core/Text";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/")({ component: Home });

function Home() {
	return (
		<AppShell contentPadding={6} height="fill" variant="wash">
			<Center axis="both" width="100%" height="100%">
				<Stack direction="vertical" gap={6} maxWidth={720} width="100%">
					<Heading level={1}>Pablo</Heading>
					<Text type="large" color="secondary">
						A warm sample surface wired to your custom Astryx theme — accent,
						typography, and semantic colors included.
					</Text>
					<Stack direction="horizontal" gap={3} vAlign="center" wrap="wrap">
						<Button variant="primary" label="Get started" />
						<Button variant="secondary" label="View tokens" />
						<Button variant="ghost" label="Docs" />
					</Stack>

					<Banner
						status="info"
						title="Theme connected"
						description="Components below inherit Pablo tokens via the Theme provider in the root route."
						isDismissable
					/>

					<Section padding={0} variant="transparent">
						<Stack direction="vertical" gap={3}>
							<Heading level={2}>Actions</Heading>
							<Stack direction="horizontal" gap={3} vAlign="center" wrap="wrap">
								<Button variant="primary" label="Primary" />
								<Button variant="secondary" label="Secondary" />
								<Button variant="ghost" label="Ghost" />
								<Button variant="destructive" label="Destructive" />
							</Stack>
						</Stack>
					</Section>

					<Section padding={0} variant="transparent">
						<Stack direction="vertical" gap={3}>
							<Heading level={2}>Status</Heading>
							<Stack direction="horizontal" gap={2} vAlign="center" wrap="wrap">
								<Badge variant="success" label="Active" />
								<Badge variant="warning" label="Pending" />
								<Badge variant="error" label="Failed" />
								<Badge variant="info" label="In review" />
								<Badge variant="neutral" label="Draft" />
							</Stack>
							<Stack direction="horizontal" gap={4} vAlign="center" wrap="wrap">
								<Stack direction="horizontal" gap={2} vAlign="center">
									<StatusDot variant="success" label="Online" />
									<Text type="supporting">Online</Text>
								</Stack>
								<Stack direction="horizontal" gap={2} vAlign="center">
									<StatusDot variant="warning" label="Degraded" />
									<Text type="supporting">Degraded</Text>
								</Stack>
								<Stack direction="horizontal" gap={2} vAlign="center">
									<StatusDot variant="accent" label="Accent" isPulsing />
									<Text type="supporting">Accent</Text>
								</Stack>
							</Stack>
						</Stack>
					</Section>

					<Section padding={0} variant="transparent">
						<Stack direction="vertical" gap={3}>
							<Heading level={2}>Surfaces</Heading>
							<Grid
								columns={{ minWidth: 240, repeat: "fit" }}
								gap={3}
								width="100%"
							>
								<Card padding={4}>
									<Stack direction="vertical" gap={2}>
										<Heading level={3}>Accent card</Heading>
										<Text type="body" color="secondary">
											Card surfaces, borders, and radius come from your theme
											tokens.
										</Text>
										<Text type="supporting" color="accent">
											Accent text token
										</Text>
									</Stack>
								</Card>
								<Card padding={4} variant="muted">
									<Stack direction="vertical" gap={2}>
										<Heading level={3}>Muted card</Heading>
										<Text type="body" color="secondary">
											Muted backgrounds help group secondary content without
											extra chrome.
										</Text>
										<Button variant="secondary" label="Open" size="sm" />
									</Stack>
								</Card>
							</Grid>
						</Stack>
					</Section>
				</Stack>
			</Center>
		</AppShell>
	);
}
