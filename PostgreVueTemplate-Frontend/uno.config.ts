import {
	defineConfig,
	presetAttributify,
	presetIcons,
	presetTypography,
	presetWebFonts,
	presetWind3,
	transformerDirectives,
	transformerVariantGroup
} from "unocss";

import { GetThemeSafeList, GetThemeStyle } from "./src/themes/themeConfig";

export default defineConfig({
	theme: {
		colors: GetThemeStyle(),
		breakpoints: {
			xs: "160px",
			sm: "320px",
			md: "640px",
			xl: "1024px",
			xxl: "1280px",
		},
	},
	presets: [
		presetWind3(),
		presetAttributify(),
		presetIcons(),
		presetTypography(),
		presetWebFonts({
			fonts: {
				// ...
			},
		}),
	],
	extendTheme(theme)
	{
		// theme.preflightBase = { };
	},
	transformers: [
		transformerDirectives(),
		transformerVariantGroup(),
	],
	safelist: GetThemeSafeList(),
	// outputToCssLayers: true
	rules: [
		[/^r-([.\d]+)$/, ([_, num]) => ({ "border-radius": `${(parseInt(num) / 4)}rem` })],
	],
});