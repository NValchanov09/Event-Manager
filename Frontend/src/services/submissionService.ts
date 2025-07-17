import { apiClient } from "@/utils/api";
import type { FilledField, GetSubmissionDto } from "@/utils/types.ts";

export async function createSubmission(eventId: number, submission: FilledField[]) {
	return apiClient.post(`/submissions/${eventId}`, { answers: submission });
}

export async function updateSubmission(eventId: number, submission: FilledField[]) {
	return apiClient.put(`/submissions/${eventId}`, { answers: submission });
}

export async function getSubmissions(eventId: string) {
	return apiClient.get<GetSubmissionDto[]>(`/submissions/${eventId}`);
}

export async function getSubmission(eventId: string) {
	return apiClient.get<GetSubmissionDto>(`/submissions/${eventId}/me`);
}

export async function cancelSubmission(eventId: string) {
	return apiClient.delete(`/submissions/${eventId}`);
}
