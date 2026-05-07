export function notify(message: string, type: "success" | "error" | "warning" | "info" = "info", timeout: number = 5000)
{
	const notificationComponent = (window as any).$notification;
	if (notificationComponent)
		notificationComponent.addNotification(message, type, timeout);
	else
		console.log(`[${type}] ${message}`);
}

export const notification = {
	success: (message: string, timeout?: number) => notify(message, "success", timeout),
	error: (message: string, timeout?: number) => notify(message, "error", timeout),
	warning: (message: string, timeout?: number) => notify(message, "warning", timeout),
	info: (message: string, timeout?: number) => notify(message, "info", timeout)
};
