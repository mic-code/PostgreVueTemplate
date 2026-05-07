import type { RouteRecordRaw } from "vue-router";

import Full from "../layouts/Full.vue";

const routes: RouteRecordRaw[] =
[
	{
		path: "/",
		component: Full,
		children: [
			{ path: "", component: () => import("../pages/Test.vue") },
			{ path: "signin", component: () => import("../pages/Signin.vue") },
			{ path: "register", component: () => import("../pages/Register.vue") },
			{ path: "forgetPass", component: () => import("../pages/ForgetPass.vue") },
			{ path: "resetPass", component: () => import("../pages/ResetPass.vue") },
			{ path: "confirmEmail", component: () => import("../pages/ConfirmEmail.vue") },
			{ path: "test", component: () => import("../pages/Test.vue") },
			{ path: "sample", component: () => import("../pages/Sample.vue") },
			{ path: "sample2", component: () => import("../pages/Sample2.vue") },
		],
	},

	// Always leave this as last one,
	// but you can also remove it
	{
		path: "/:catchAll(.*)*",
		component: () => import("../pages/NotFound.vue"),
	},
];

export default routes;
