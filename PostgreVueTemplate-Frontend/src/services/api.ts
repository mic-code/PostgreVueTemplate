import { useAppStore } from "../stores/appStore";
import { useAuthStore } from "../stores/authStore";

export function ClearToken()
{
	tokenReady = false;
	sessionStorage.removeItem("token");
	sessionStorage.removeItem("tokenMeta");
}

export function SetToken(tokenObject)
{
	tokenReady = true;
	sessionStorage.setItem("token", tokenObject.token);
	sessionStorage.setItem("tokenMeta", tokenObject.meta);
}

export function GetToken()
{
	return sessionStorage.getItem("token");
}

let gettingToken: boolean;
let tokenReady: boolean;

export async function TryGetNewToken(autoRedirect: boolean = true): Promise<boolean>
{
	if(!gettingToken)
	{
		gettingToken = true;
		tokenReady = false;

		const store = useAuthStore();
		const response = await fetch("/api/Auth/GetToken");
		if (response.status == 200)
		{
			const tokenObject = await response.json();
			SetToken(tokenObject);
			gettingToken = false;
			store.isLoggedIn = true;
			return true;
		}

		if(autoRedirect)
			store.signout();
		tokenReady = false;
		gettingToken = false;
	}
	else
	{
		while (gettingToken)
			await sleep(100);
		return tokenReady;
	}

	return false;
}

export async function Get(url: string): Promise<Response>
{
	let retry = true;
	let response;

	while (retry)
	{
		retry = false;
		response = await fetch("/api/" + url, {
			method: "GET",
			headers: {
				"Authorization": "Bearer " + sessionStorage.getItem("token")
			}
		});

		if (response.status == 401)
		{
			if (await TryGetNewToken())
				retry = true;
		}
		else
		{
			break;
		}
	}
	return response;
}

export async function GetDirect(url: string): Promise<Response>
{
	const response = await fetch("/api/" + url, {
		method: "GET",
		headers: {
			"Authorization": "Bearer " + sessionStorage.getItem("token")
		}
	});

	return response;
}

export async function PostJsonDirect(url: string, json): Promise<Response>
{
	return await fetch("/api/" + url, {
		method: "POST",
		headers: {
			"Accept": "application/json",
			"Content-Type": "application/json",
			"Authorization": "Bearer " + sessionStorage.getItem("token")
		},
		body: JSON.stringify(json)
	});
}

export async function GetResult(url: string): Promise<[number, any]>
{
	const response = await Get(url);
	if(isServerError(response.status))
	{
		const appStore = useAppStore();
		appStore.serverError = true;
	}

	try
	{
		const json = await response.json();
		return [response.status, json];
	}
	catch
	{
		return [response.status, response.statusText];
	}
}

export async function GetResultDirect(url: string): Promise<[number, any]>
{
	const response = await GetDirect(url);
	try
	{
		const json = await response.json();
		return [response.status, json];
	}
	catch
	{
		return [response.status, response.statusText];
	}
}

export async function PostJsonResult(url: string, payload: any): Promise<[number, any]>
{
	const response = await PostJson(url, payload);
	if(isServerError(response.status))
	{
		const appStore = useAppStore();
		appStore.serverError = true;
	}
	try
	{
		const json = await response.json();
		return [response.status, json];
	}
	catch
	{
		return [response.status, response.statusText];
	}
}

export async function PostJsonDirectResult(url: string, payload: any): Promise<[number, any]>
{
	const response = await PostJsonDirect(url, payload);
	try
	{
		const json = await response.json();
		return [response.status, json];
	}
	catch
	{
		return [response.status, { Result: response.statusText }];
	}
}

export async function PostJson(url: string, json): Promise<Response>
{
	let retry = true;
	let response;

	while (retry)
	{
		// const controller = new AbortController();
		// const timeoutId = setTimeout(() => controller.abort(), 10000);
		retry = false;
		response = await fetch("/api/" + url, {
			method: "POST",
			// signal: controller.signal,
			headers: {
				"Accept": "application/json",
				"Content-Type": "application/json",
				"Authorization": "Bearer " + sessionStorage.getItem("token")
			},
			body: JSON.stringify(json)
		});

		// clearTimeout(timeoutId);

		if (response.status == 401)
		{
			if (await TryGetNewToken())
				retry = true;
		}
		else
		{
			break;
		}
	}
	return response;
}

export async function PostSignboard(url: string, instanceHash: string, virtualBoard: string, formData)
{
	let retry = true;
	let response;

	while (retry)
	{
		retry = false;
		response = await fetch("/api/" + url, {
			method: "POST",
			headers: {
				"InstanceHash": instanceHash,
				"VirtualBoard": virtualBoard
			},
			body: formData
		});

		if (response.status == 401)
		{
			if (await TryGetNewToken())
				retry = true;
		}
		else
		{
			break;
		}
	}
	return response;
}

export async function PostUpload(url: string, boothID: number, formData)
{
	let retry = true;
	let response;

	while (retry)
	{
		retry = false;
		response = await fetch("/api/" + url, {
			method: "POST",
			headers: {
				"BoothID": boothID.toString(),
				"Authorization": "Bearer " + sessionStorage.getItem("token")
			},
			body: formData
		});

		if (response.status == 401)
		{
			if (await TryGetNewToken())
				retry = true;
		}
		else
		{
			break;
		}
	}
	return response;
}

export async function PostUploadPerBoothXML(request: XMLHttpRequest, url: string, boothID: number, formData: FormData)
{
	console.log("PostUploadPerBoothXML" + boothID);
	request.open("POST", "/api/" + url);
	request.setRequestHeader("BoothID", boothID.toString());
	request.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("token"));
	request.send(formData);
}

export async function PostUploadPerUserXML(request: XMLHttpRequest, url: string, formData: FormData)
{
	console.log("PostUploadPerUserXML");
	request.open("POST", "/api/" + url);
	request.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("token"));
	request.send(formData);
}

export async function PostUploadXML(request: XMLHttpRequest, url: string, formData: FormData)
{
	request.open("POST", "/api/" + url);
	request.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("token"));
	request.send(formData);
}

export async function OpenPostUploadXML(request: XMLHttpRequest, url: string)
{
	request.open("POST", "/api/" + url);
	request.setRequestHeader("Authorization", "Bearer " + sessionStorage.getItem("token"));
}

export async function Post(url: string): Promise<Response>
{
	let retry = true;
	let response;

	while (retry)
	{
		retry = false;
		response = await fetch("/api/" + url, {
			method: "POST",
			headers: {
				"Authorization": "Bearer " + sessionStorage.getItem("token")
			},
		});

		if (response.status == 401)
		{
			if (await TryGetNewToken())
				retry = true;
		}
		else
		{
			break;
		}
	}
	return response;
}

export function sleep(ms)
{
	return new Promise(resolve => setTimeout(resolve, ms));
}

export function isServerError(status)
{
	return status == 500 || status == 502 || status == 504;
}