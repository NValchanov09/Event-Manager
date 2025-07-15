import { createRouter, createWebHistory } from "vue-router";

import { useUserStore } from "@/stores/userStore";
import { useUIStore } from "@/stores/uiStore";
import { useCurrentEventStore } from "@/stores/currentEventStore";

const HomeView = () => import("@/views/HomeView.vue");
const EventsView = () => import("@/views/EventsView.vue");
const EventDetailsView = () => import("@/views/EventDetailsView.vue");
const JoinedEventsView = () => import("@/views/JoinedEventsView.vue");
const CreateEvent = () => import("@/views/CreateEventView.vue");
const EditEventView = () => import("@/views/EditEventView.vue");
const UsersView = () => import("@/views/UsersView.vue");
const LogInView = () => import("@/views/LogInView.vue");
const NotFoundView = () => import("@/views/NotFoundView.vue");
const SubmissionsView = () => import("@/views/SubmissionsView.vue");

const router = createRouter({
	history: createWebHistory(import.meta.env.BASE_URL),
	routes: [
		{
			path: "/",
			name: "home",
			component: HomeView,
		},
		{
			path: "/events",
			name: "events",
			meta: { requiresAuth: true },
			children: [
				{
					path: "",
					name: "events-list",
					component: EventsView,
				},
				{
					path: ":id",
					name: "event-details",
					component: EventDetailsView,
				},
				{
					path: "joined",
					name: "joined-events",
					component: JoinedEventsView,
				},
				{
					path: "create",
					name: "create-event",
					component: CreateEvent,
					meta: { requiresAdmin: true },
				},
				{
					path: ":id/edit",
					name: "edit-event",
					component: EditEventView,
					meta: { requiresAdmin: true },
				},
				{
					path: ":id/submissions",
					name: "submissions",
					component: SubmissionsView,
					meta: { requiresAuth: true, requiresAdmin: true },
				},
			],
		},
		{
			path: "/users",
			name: "users",
			component: UsersView,
			meta: { requiresAuth: true, requiresAdmin: true },
		},
		{
			path: "/login",
			name: "login",
			component: LogInView,
			meta: { requiresGuest: true },
		},
		{
			path: "/not-found",
			name: "not-found",
			component: NotFoundView,
		},
		{
			path: "/:pathMatch(.*)*",
			redirect: { name: "not-found" },
		},
	],
});

router.beforeEach(async (to, from, next) => {
	const userStore = useUserStore();
	const { triggerToast } = useUIStore();
	const { clearCurrentEvent } = useCurrentEventStore();

	// Clear current event when navigating away from create/edit event pages
	if (from.name === "create-event" || from.name === "edit-event") {
		clearCurrentEvent();
	}

	// Handle token refresh
	if (userStore.refreshToken && !userStore.accessToken) {
		try {
			const success = await userStore.refreshAccessToken();

			if (!success) {
				triggerToast(
					"Неуспешно обновяване на токена. Моля, впишете се отново.",
					"error"
				);

				return next("/login");
			}
		} catch {
			triggerToast(
				"Неуспешно обновяване на токена. Моля, впишете се отново.",
				"error"
			);

			return next("/login");
		}
	}

	await userStore.fetchUser();

	const requiresAuth = to.matched.some(record => record.meta.requiresAuth);
	const requiresGuest = to.matched.some(record => record.meta.requiresGuest);
	const requiresAdmin = to.matched.some(record => record.meta.requiresAdmin);

	if (requiresAuth && !userStore.isAuthenticated) {
		triggerToast(
			"Трябва да бъдете вписани, за да имате достъп до тази страница!",
			"warning"
		);

		return next("/login");
	} else if (requiresGuest && userStore.isAuthenticated) {
		triggerToast(
			"Вече сте вписани и не можете да достъпите тази страница!",
			"warning"
		);

		return next("/");
	}

	if (requiresAdmin && !userStore.isAdmin) {
		triggerToast(
			"Нуждаете се от администраторски права, за да достъпите тази страница!",
			"warning"
		);

		return next("/");
	}

	next();
});

export default router;
