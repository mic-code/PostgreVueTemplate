import { useColorMode } from "@vueuse/core";
import { defineStore } from "pinia";
import { ref, watch } from "vue";

export const useAppStore = defineStore("app", {
	state: () =>
	{
		const debugMediaItem = ref(sessionStorage.getItem("debugMediaItem") == "true");
		watch(debugMediaItem, ()=>{sessionStorage.setItem("debugMediaItem", debugMediaItem.value.toString( )); });

		const isLocal = location.hostname == "localhost";
		const isDev = location.hostname == "dev.im-booth.com" || isLocal;
		const colorMode = useColorMode({
			attribute: "theme",
			modes: {
				light: "light",
				dark: "dark",
			},
		});
		const state = {
			colorMode,
			isLoading: false,
			isStudio: location.hostname == "global.im-studio.live",
			isLocal: isLocal,
			isDev: isDev,
			debugMediaItem: debugMediaItem,
			serverError: false,
			stripAPIKey: isDev ?
				"pk_test_51PEp2iFuvG2O7Nutt34jXfamkSzk8YaiNFEVCAIpziKBBMmrsTSIHDXb40cb3fc8eRRQCcD3nnqE1VjS5dD7JGDW00jsDfVpGo" :
				"pk_live_51PEp2iFuvG2O7NutlIeMkybksk7WWn6nqahsq84VoBhBHlrLNMhw7XcaAPaS0AUCwDN8wemDvLFV9j7pTkRswypj00jzDF7Yv0"
		};

		return state;
	},
	actions: {
		setLoading(isLoading: boolean)
		{
			this.isLoading = isLoading;
		}
	},
});
