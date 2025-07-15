import { format } from "date-fns";
import { TZDate, tz } from "@date-fns/tz";

/**
 * Convert UTC datetime string to local datetime string for datetime-local input
 * @param utcString - UTC datetime string from server (ISO format)
 * @returns Local datetime string in YYYY-MM-DDTHH:mm format
 */
export const utcToLocal = (utcString: string): string => {
	if (!utcString) return "";

	try {
		// Get user's local timezone
		const localTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;

		// Ensure the UTC string has proper UTC indicator
		const properUtcString = utcString.endsWith("Z")
			? utcString
			: utcString + "Z";

		// Create proper UTC date and convert to local for datetime-local input
		const utcDate = new Date(properUtcString);

		// Format with timezone conversion for datetime-local input
		return format(utcDate, "yyyy-MM-dd'T'HH:mm", {
			in: tz(localTimeZone),
		});
	} catch (error) {
		console.error("Error converting UTC to local:", error);
		return "";
	}
};

/**
 * Convert local datetime string to UTC datetime string for server
 * @param localString - Local datetime string from datetime-local input (YYYY-MM-DDTHH:mm)
 * @returns UTC datetime string in ISO format
 */
export const localToUtc = (localString: string): string => {
	if (!localString) return "";

	try {
		// Get user's local timezone
		const localTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;

		// Parse the local datetime string components
		const [datePart, timePart] = localString.split("T");
		const [year, month, day] = datePart.split("-").map(Number);
		const [hour, minute] = timePart.split(":").map(Number);

		// Create TZDate in local timezone
		// This interprets the date/time as being in the local timezone
		const localDate = new TZDate(
			year,
			month - 1,
			day,
			hour,
			minute,
			localTimeZone
		);

		// Convert to UTC ISO string
		return localDate.toISOString();
	} catch (error) {
		console.error("Error converting local to UTC:", error);
		return "";
	}
};

/**
 * Get current date/time in local format for datetime-local min attribute
 * @returns Current local datetime string in YYYY-MM-DDTHH:mm format
 */
export const getCurrentLocalDateTime = (): string => {
	const localTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
	const now = new TZDate(new Date(), localTimeZone);
	return format(now, "yyyy-MM-dd'T'HH:mm", { in: tz(localTimeZone) });
};

/**
 * Format UTC datetime string for display (converts to local time)
 * @param utcString - UTC datetime string from server
 * @returns Formatted local datetime string for display
 */
export const formatDateTime = (utcString: string) => {
	if (!utcString) return "";

	try {
		// Get user's local timezone
		const localTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;

		// Ensure the UTC string has proper UTC indicator
		const properUtcString = utcString.endsWith("Z")
			? utcString
			: utcString + "Z";

		// Create proper UTC date and format for display in local timezone
		const utcDate = new Date(properUtcString);

		// Format with timezone conversion for display
		return format(utcDate, "EEEE, MMMM d, yyyy 'at' HH:mm", {
			in: tz(localTimeZone),
		});
	} catch (error) {
		console.error("Error formatting date:", error);
		return "";
	}
};
