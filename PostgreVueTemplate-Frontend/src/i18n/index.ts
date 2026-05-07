import { createI18n } from "vue-i18n";
import en from "./en.json" assert { type: "json" };
import hk from "./zh-HK.json" assert { type: "json" };

const messages = {
	"en": en,
	"zh-HK": hk
};

let defaultLocale = sessionStorage.getItem("lang");
if(defaultLocale == null)
	defaultLocale = "en";

export const i18n = createI18n({
	legacy: false, // Vuetify does not support the legacy mode of vue-i18n
	locale: defaultLocale,
	messages,
});