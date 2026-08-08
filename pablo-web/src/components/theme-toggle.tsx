import { Moon, Sun } from "@phosphor-icons/react";
import { useEffect, useState } from "react";

import { Button } from "@/components/ui/button";

const THEME_KEY = "theme";

type Theme = "light" | "dark";

function readTheme(): Theme {
	const stored = localStorage.getItem(THEME_KEY);
	if (stored === "light" || stored === "dark") {
		return stored;
	}
	return window.matchMedia("(prefers-color-scheme: dark)").matches
		? "dark"
		: "light";
}

function applyTheme(theme: Theme) {
	document.documentElement.classList.toggle("dark", theme === "dark");
	localStorage.setItem(THEME_KEY, theme);
}

export function ThemeToggle() {
	const [theme, setTheme] = useState<Theme>("light");

	useEffect(() => {
		setTheme(readTheme());
	}, []);

	return (
		<Button
			variant="ghost"
			size="icon"
			aria-label={
				theme === "dark" ? "Switch to light mode" : "Switch to dark mode"
			}
			onClick={() => {
				const next = theme === "dark" ? "light" : "dark";
				applyTheme(next);
				setTheme(next);
			}}
		>
			<Sun className="hidden dark:block" />
			<Moon className="block dark:hidden" />
		</Button>
	);
}
