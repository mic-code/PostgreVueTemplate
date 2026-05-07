import { defineStore } from "pinia";
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from "@microsoft/signalr";
import { GetToken, TryGetNewToken } from "../services/api";

let reconnectTimeout = null;

export class UserHubChannel
{
	static Asset = "Asset";
	static Scene = "Scene";
	static Worker = "Worker";
	static Job = "Job";
	static Chat = "Chat";
}

export class UserHubMethods
{
	static Nexus = "Nexus";
	static SubscribeSceneChange = "SubscribeSceneChange";
}

export const useRealTimeStore = defineStore("realtime", {
	state: () =>
	{
		return {
			connection: null as HubConnection,
			isConnected: false
		};
	},
	actions: {
		async startService()
		{
			if(GetToken() == null)
			{
				await TryGetNewToken(false);

				if(GetToken() == null)
					return;
			}

			this.applyLock();
			// console.log("start realtime service");

			if(this.connection == null || this.connection.state == HubConnectionState.Disconnected)
			{
				this.connection = new HubConnectionBuilder()
					.withUrl("/api/userHub", { accessTokenFactory: () => sessionStorage.getItem("token") })
					.withAutomaticReconnect({
						nextRetryDelayInMilliseconds: retryContext =>
						{
							return 5000;
						}
					})
					.withStatefulReconnect()
					.configureLogging(LogLevel.None)
					.build();

				// this.connection.on(UserHubChannel.Asset, (oldValue: Asset, newValue: Asset) =>
				// {

				// });

				this.connection.onclose(()=>
				{
					this.isConnected = false;
					// console.log("onclose");
				});
				this.connection.onreconnecting(()=>
				{
					this.isConnected = false;
					// console.log("onreconnecting");
				});
				this.connection.onreconnected(()=>
				{
					this.isConnected = true;
					// console.log("onreconnected");
				});
				this.connect();
			}

			window.onfocus = this.checkConnection;
		},
		async stopService()
		{
			console.log("stop realtime service");
			if(this.connection.state == HubConnectionState.Connected)
				this.connection.stop();

			this.connection = null;
			clearTimeout(reconnectTimeout);
		},
		checkConnection()
		{
			if(this.connection != null && this.connection.state == HubConnectionState.Disconnected)
				if(reconnectTimeout == null)
				{
					// console.log("onfocus reconnect");
					this.connect();
				}

			if(this.connection == null)
				this.startService();
		},
		connect()
		{
			// console.log("connecting");
			this.connection.start()
				.then(()=>
				{
					this.isConnected = true;
					// console.log("connected");
					clearTimeout(reconnectTimeout);
				})
				.catch((err) =>
				{
					this.isConnected = false;
					if(!err.message.includes("401"))
					{
						// this.stopService();
						reconnectTimeout = setTimeout(this.connect, 3000);
					}
				});
		},
		applyLock()
		{
			let lockResolver;
			if (navigator && navigator.locks && navigator.locks.request)
			{
				const promise = new Promise((res) =>
				{
					lockResolver = res;
				});

				navigator.locks.request("unique_lock_name", { mode: "shared" }, () =>
				{
					return promise;
				});
			}
		},
		test()
		{
			this.connection.invoke("test", 2);
		},
		invoke(method, ...args: any[])
		{
			this.connection.invoke(method, ...args);
		},
		subscribeToSceneChange(sceneId: boolean, callback: () => void)
		{
			if(!this.isConnected)
				setTimeout(()=>this.subscribeToSceneChange(sceneId, callback), 1000);
			else
			{
				console.log("subscribeToSceneChange " + sceneId);
				this.connection.invoke(UserHubMethods.SubscribeSceneChange, sceneId);
			}
		},
		unsubscribeToSceneChange()
		{

		}
	},
});