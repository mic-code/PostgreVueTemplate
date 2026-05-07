import type { Router } from "vue-router";

export function IsSubPathOf(route, target)
{
	const path = route.path + route.hash;
	return path.indexOf(target) === 0;
}

export function IsSubPathOfArray(route, targets: Array<string>)
{
	const path = route.path + route.hash;
	let isActive = false;
	targets.forEach(element =>
	{
		if(path.indexOf(element) === 0)
			isActive = true;
	});
	return isActive;
}

export function gotoMachines(router: Router, user: string)
{
	router.push({ path: "/machines", query: { user } } );
}

export function gotoConfig(router: Router, user: string)
{
	router.push({ path: "/booths", query: { user } } );
}