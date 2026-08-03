import { Button } from "@astryxdesign/core/Button";
import { VStack } from "@astryxdesign/core/Layout";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/")({ component: Home });

function Home() {
	return (
		<VStack gap={2}>
			<Button label="Hello Astryx" onClick={() => alert("Hi!")} />
		</VStack>
	);
}
