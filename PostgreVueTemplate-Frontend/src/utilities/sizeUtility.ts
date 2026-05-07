export const defaultSize = 2;

export function computeFontSize(size, fontSize)
{
	if(fontSize)
		return fontSize;
	else
		return size / 2 + "rem";
}

export function computeHeight(size, height)
{
	if(height)
		return height;
	else
		return size + "rem";
}

export function computeRound(size, round)
{
	if(round)
		return round;
	else
		return size / 2 + "rem";
}