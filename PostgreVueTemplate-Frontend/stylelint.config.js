// import { rules } from "eslint-plugin-vue";

export default {
	extends: [
		"stylelint-config-standard",
		"@stylistic/stylelint-config"
	],
	overrides: [
		{
			files: ["*.scss", "**/*.scss"],
			extends: ["stylelint-config-standard-scss"]
		},
		{
			files: ["*.vue", "**/*.vue"],
			extends: [
				"stylelint-config-standard-scss",
				"stylelint-config-standard-vue/scss"
			]
		}
	],
	rules: {
		"@stylistic/indentation": "tab",
		"@stylistic/selector-list-comma-newline-after": null,
		"selector-class-pattern": null,
		"color-hex-length": null,
		"declaration-property-value-no-unknown": null,
		// "@stylistic/indentation": 2
	}
};