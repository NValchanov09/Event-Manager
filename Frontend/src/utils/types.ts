export interface FormField {
	id: number;
	type: "checkbox" | "text" | "radio";
	name: string;
	required: boolean;
	options: string[];
}

export interface Event {
	id: number;
	name: string;
	description: string;
	date: string;
	signUpDeadline: string;
	location: string;
	fields: FormField[];
	spotsLeft: number;
	peopleLimit?: number;
	userSignedUp: boolean;
}

export interface CreateEventDto {
	name: string;
	description: string;
	date: string;
	signUpDeadline?: string;
	location: string;
	fields: FormField[];
	peopleLimit?: number;
}

export interface FilledField {
	id: number;
	name: string;
	options: string[];
}

export interface GetSubmissionDto {
	submissions: FilledField[];
	date: string;
	email: string;
	userId: string;
}

export interface CreateEventResponse {
	id: string;
}

export interface User {
	id: string;
	email: string;
	roles: string[];
}

export interface UpdateEventDto {
	name?: string;
	description?: string;
	date?: string;
	signUpDeadline?: string;
	location?: string;
	fields?: FormField[];
	peopleLimit?: number;
}
