import { apiClient } from "@/utils/api";
import type {
	CreateEventDto,
	CreateEventResponse,
	Event,
	UpdateEventDto,
} from "@/utils/types";

export function createEvent(dto: CreateEventDto) {
	return apiClient.post<CreateEventResponse>("/events", dto);
}

export function getEvents() {
	return apiClient.get<Event[]>("/events");
}

export function getJoinedEvents() {
	return apiClient.get<Event[]>("/events/joined");
}

export function getEventById(id: string) {
	return apiClient.get<Event>(`/events/${id}`);
}

export function updateEvent(id: string, dto: UpdateEventDto) {
	return apiClient.put<Event>(`/events/${id}`, dto);
}

export function deleteEvent(id: string) {
	return apiClient.delete(`/events/${id}`);
}

export interface EventsFilterParams {
	fromDate?: string;
	toDate?: string;
	activeOnly?: boolean;
	sortDescending?: boolean;
	alphabetical?: boolean;
}

export function getEventsWithFilters(params?: EventsFilterParams) {
	if (!params || Object.keys(params).length === 0) {
		return getEvents();
	}

	const searchParams = new URLSearchParams();

	if (params.fromDate) searchParams.append("fromDate", params.fromDate);
	if (params.toDate) searchParams.append("toDate", params.toDate);
	if (params.activeOnly !== undefined)
		searchParams.append("activeOnly", params.activeOnly.toString());
	if (params.sortDescending !== undefined)
		searchParams.append("sortDescending", params.sortDescending.toString());
	if (params.alphabetical !== undefined)
		searchParams.append("alphabetical", params.alphabetical.toString());

	const queryString = searchParams.toString();
	return apiClient.get<Event[]>(`/events?${queryString}`);
}

export function getJoinedEventsWithFilters(params?: EventsFilterParams) {
	if (!params || Object.keys(params).length === 0) {
		return getJoinedEvents();
	}

	const searchParams = new URLSearchParams();

	if (params.fromDate) searchParams.append("fromDate", params.fromDate);
	if (params.toDate) searchParams.append("toDate", params.toDate);
	if (params.activeOnly !== undefined)
		searchParams.append("activeOnly", params.activeOnly.toString());
	if (params.sortDescending !== undefined)
		searchParams.append("sortDescending", params.sortDescending.toString());
	if (params.alphabetical !== undefined)
		searchParams.append("alphabetical", params.alphabetical.toString());

	const queryString = searchParams.toString();
	return apiClient.get<Event[]>(`/events/joined?${queryString}`);
}
