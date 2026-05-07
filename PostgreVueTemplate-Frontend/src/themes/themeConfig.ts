import { themeColorNames } from "./themeNames";

export function GetThemeColorNames()
{
	return themeColorNames;
}

export function GetThemeSafeList(): string[]
{
	const themeSafeList = [];
	for	(let i = 0;i < themeColorNames.length;i++)
	{
		themeSafeList.push(`text-${themeColorNames[i]}`);
		themeSafeList.push(`bg-${themeColorNames[i]}`);
	}
	return themeSafeList;
}

export function GetThemeStyle()
{
	let themeStyle = {};
	for	(let i = 0;i < themeColorNames.length;i++)
		themeStyle[themeColorNames[i]] = (`var(--${themeColorNames[i]})`);
	return themeStyle;
}

export const onColorNames = ["success", "warning", "danger"];

export class ThemeColor
{
	static primary = "primary";
	static secondary = "secondary";
	static success = "success";
	static warning = "warning";
	static danger = "danger";
}
