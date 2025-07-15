/**
 * Utility functions for standardized error handling across the application
 */

export type ApiError = Error & {
	response?: { status: number; data: unknown };
	statusCode?: number;
};

/**
 * Extract a user-friendly error message from different error formats
 * @param error - The error object to extract message from
 * @param fallbackMessage - Default message if no specific error is found
 * @returns User-friendly error message
 */
export const extractErrorMessage = (
	error: unknown,
	fallbackMessage = "Възникна неочаквана грешка"
): string => {
	// Check for our enhanced API error format first
	if (error && typeof error === "object" && "response" in error) {
		const apiError = error as ApiError;
		if (
			apiError.response?.data &&
			typeof apiError.response.data === "object"
		) {
			const errorData = apiError.response.data as Record<string, unknown>;

			// Try to extract error message in order of preference
			if ("error" in errorData && typeof errorData.error === "string") {
				return errorData.error;
			}
			if (
				"message" in errorData &&
				typeof errorData.message === "string"
			) {
				return errorData.message;
			}
		}
	}

	// Check for standard Error object
	if (error instanceof Error && error.message) {
		return error.message;
	}

	// Check for string error
	if (typeof error === "string") {
		return error;
	}

	// Fallback to provided message
	return fallbackMessage;
};
