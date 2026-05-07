import { Get, GetResult, GetResultDirect, PostJsonDirectResult } from "./api";

export class Account
{
	UserName: string;
	Roles: string[];
	EmailConfirmed: boolean;
	Credit: number;
}

export async function Signout()
{
	await Get("Auth/Signout");
}

export async function GetRoles(): Promise<[number, Account]>
{
	return await GetResult("Auth/GetUser");
}

export async function GetRolesDirect(): Promise<[number, Account]>
{
	return await GetResultDirect("Auth/GetUser");
}

export async function Register(Email, Password)
{
	return await PostJsonDirectResult("Auth/Register", { Email, Password });
}

export async function Signin(Email, Password)
{
	return await PostJsonDirectResult("Auth/Signin", { Email, Password });
}

export async function ConfirmEmail(Email, Token)
{
	return await PostJsonDirectResult("Auth/ConfirmEmail", { Email, Token });
}

export async function ForgetPassword(Email)
{
	return await PostJsonDirectResult("Auth/ForgetPassword", { Email });
}

export async function ResetPassword(Email, Password, Token)
{
	return await PostJsonDirectResult("Auth/ResetPassword", { Email, Password, Token });
}
