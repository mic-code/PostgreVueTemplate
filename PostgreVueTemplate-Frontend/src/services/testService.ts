import { GetResult } from "./api";

export async function IsUp()
{
	return await GetResult("Test/IsUp");
}

export async function IsUpAuth()
{
	return await GetResult("Test/IsUpAuth");
}

export async function IsUpClaim()
{
	return await GetResult("Test/IsUpClaim");
}