import { defineConfig } from "vite";
import Vue from "@vitejs/plugin-vue";
import mkcert from "vite-plugin-mkcert";
import Icons from "unplugin-icons/vite";
import IconsResolver from "unplugin-icons/resolver";
import Components from "unplugin-vue-components/vite";
import TurboConsole from "unplugin-turbo-console/vite";
import { presetAttributify, presetWind3 } from "unocss";
import UnoCSS from "unocss/vite";

// import { fileURLToPath, URL } from "node:url";

export default defineConfig({
	plugins: [
		Vue(),
		Components({
			resolvers: [
				IconsResolver(),
			],
		}),
		Icons(),
		UnoCSS({ presets: [
			presetWind3(),
			presetAttributify(),
		] }),
		TurboConsole({}),
		mkcert()
	],
	define: { "process.env": {} },
	server: {
		port: 3050,
		proxy: {
			"/api": {
				target: "http://localhost:8088",
				changeOrigin: true,
				ws: true
			}
		}
	},
	preview: {
		port: 443,
	}
});
