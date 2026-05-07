import { onColorNames } from "../themes/themeConfig";
import { themeColorNames } from "../themes/themeNames";

export function GetTextColor(bgColor)
{
	if(!bgColor)
		return "onsurface";

	if(bgColor == "oncolor")
		return "oncolor";

	if(bgColor == "primary")
		return "onprimary";
	if(bgColor == "secondary")
		return "onsecondary";

	return onColorNames.includes(bgColor.toLowerCase()) ? "oncolor" : "onsurface";
}

export function GetColorFromName(colorName: string)
{
	let finalColor = "var(--primary)";
	if (colorName)
	{
		if (themeColorNames.includes(colorName))
			finalColor = `var(--${colorName})`;
		else
			finalColor = colorName;
	}

	return finalColor;
}