import { defineStore } from "pinia";
import { ClearToken, TryGetNewToken } from "../services/api";
import { router } from "../router";
import { GetRoles, GetRolesDirect, Signin, Signout } from "../services/authService";
import { useRealTimeStore } from "./realtimeStore";
// import { useRealTimeStore } from "./realtimeStore";

export const useAuthStore = defineStore("auth", {
	state: () => (
		{
			userName: null as string,
			credit: -1 as number,
			isLoggedIn: false,
			isAdmin: false,
			signinError: null as string,
			roles: []
		}),
	actions: {
		async signin(email, password)
		{
			this.signinError = null;

			const [status, result] = await Signin(email, password);
			this.isLoggedIn = status == 200;

			if(this.isLoggedIn)
			{
				this.userName = email;
				await TryGetNewToken();
				this.roles = await GetRoles();

				if(this.roles != null)
					this.isAdmin = this.roles.filter((role)=>role == "Admin").length > 0;

				// const realtimeStore = useRealTimeStore();
				// realtimeStore.startService();
			}
			else
			{
				this.signinError = result.Result;
			}
		},
		async checkLogin()
		{
			this.isLoggedIn = false;
			const [status, user] = await GetRoles();
			if(status == 200)
				this.updateAuth(user);
		},
		async checkLoginNoRedirect()
		{
			this.isLoggedIn = false;
			const [status, user] = await GetRolesDirect();
			if(status == 200)
				this.updateAuth(user);
		},
		updateAuth(user)
		{
			this.roles = user.Roles;
			this.userName = user.UserName;
			this.isLoggedIn = true;

			if(this.roles != null)
				this.isAdmin = this.roles.filter((role)=>role == "Admin").length > 0;
		},
		async signout()
		{
			this.roles = [];
			this.userName = null;
			this.credit = -1;
			this.isLoggedIn = false;
			this.signinError = null;
			this.isAdmin = false;

			router.push({ path: "/signin" });
			ClearToken();

			const realtimeStore = useRealTimeStore();
			realtimeStore.stopService();

			await Signout();
		}
	},
});
