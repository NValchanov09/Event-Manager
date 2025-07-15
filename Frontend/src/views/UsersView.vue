<script setup lang="ts">
import UserIcon from "@/components/icons/UserIcon.vue";
import { RouterLink } from "vue-router";
import { ref, onMounted } from "vue";
import ConfirmationComponent from "@/components/ConfirmationComponent.vue";
import { useUIStore } from "@/stores/uiStore";
import LoaderComponent from "@/components/LoaderComponent.vue";
import { useAsync } from "@/composables/useAsync";
import { getUsers, addAdmin, removeAdmin } from "@/services/usersService";
import type { User } from "@/utils/types";

// Utility function to check if a user is an admin
const isUserAdmin = (user: User): boolean => {
	return user.roles?.includes("Administrator") ?? false;
};

const uiStore = useUIStore();
const showConfirmDialog = ref(false);
const userToUpdate = ref<{ userId: string; isAdmin: boolean } | null>(null);
const confirmMessage = ref("");

const { execute, data: users, loading, error } = useAsync(getUsers);

const updateUserAdminStatus = async (userId: string, isAdmin: boolean) => {
	try {
		if (isAdmin) {
			await addAdmin(userId);
		} else {
			await removeAdmin(userId);
		}

		await execute(); // Refresh the users list
	} catch (err) {
		throw err;
	}
};

const promptConfirm = (userId: string, willBeAdmin: boolean) => {
	userToUpdate.value = { userId, isAdmin: willBeAdmin };

	confirmMessage.value = willBeAdmin
		? `Сигурни ли сте, че искате да направите този потребител администратор?`
		: `Сигурни ли сте, че искате да премахнете администраторските права на този потребител?`;

	showConfirmDialog.value = true;
};

const handleConfirm = async (confirmed: boolean) => {
	showConfirmDialog.value = false;

	if (confirmed && userToUpdate.value) {
		try {
			await updateUserAdminStatus(
				userToUpdate.value.userId,
				userToUpdate.value.isAdmin
			);

			uiStore.triggerToast(
				userToUpdate.value.isAdmin
					? "Потребителят беше направен администратор успешно!"
					: "Администраторските права бяха премахнати успешно!",
				"success"
			);
		} catch (err) {
			uiStore.triggerToast(
				"Неуспешна промяна на ролята. Моля, опитайте отново.",
				"error"
			);

			console.error("Error updating user role:", err);
		}
	}

	userToUpdate.value = null;
};

onMounted(execute);
</script>

<template>
	<div class="p-5">
		<div v-if="loading"><LoaderComponent /></div>
		<div v-else-if="error" class="p-10 text-center text-red">
			Възникна грешка: {{ error }}
		</div>
		<div v-else-if="!users || users.length === 0">
			<div class="p-10 text-center text-white">
				Няма намерени потребители.
			</div>
		</div>
		<template v-else>
			<table
				class="min-w-4 divide-y divide-gray-200 ml-auto mr-auto w-1/2">
				<thead class="bg-gray-800">
					<tr>
						<th
							scope="col"
							class="px-6 py-3 text-left text-xs font-medium text-yellow uppercase tracking-wider">
							Имейл
						</th>
						<th
							scope="col"
							class="py-3 text-left text-xs font-medium text-yellow uppercase tracking-wider">
							Роля
						</th>
						<th
							scope="col"
							class="px-6 py-3 text-left text-xs font-medium text-yellow uppercase tracking-wider">
							Действия
						</th>
					</tr>
				</thead>
				<tbody class="bg-gray-700 divide-y divide-gray-200 y-600">
					<tr
						v-for="user in users"
						:key="user.id"
						class="hover:bg-gray-600 transition-colors">
						<td class="px-6 py-4 whitespace-nowrap">
							<RouterLink
								:to="''"
								class="flex items-center hover:text-yellow transition-colors cursor-default">
								<UserIcon class="flex-shrink-0" />
								<span class="ml-3 text-white"
									>{{ user.email }}
								</span>
							</RouterLink>
						</td>
						<td class="py-4 whitespace-nowrap">
							<RouterLink
								:to="''"
								class="flex items-start hover:text-yellow transition-colors cursor-default">
								<span
									v-if="isUserAdmin(user)"
									class="text-white"
									>Администратор</span
								>
								<span v-else class="text-white"
									>Потребител</span
								>
							</RouterLink>
						</td>
						<td class="px-6 py-4 whitespace-nowrap">
							<RouterLink
								:to="''"
								class="flex items-start justify-between hover:text-yellow transition-colors">
								<div class="flex justify-end gap-2">
									<button
										v-if="!isUserAdmin(user)"
										@click="promptConfirm(user.id, true)"
										class="bg-cyan text-dark-grey px-3 py-1 rounded-md hover:opacity-90 transition-colors font-medium cursor-pointer whitespace-nowrap">
										Направи админ
									</button>
									<button
										v-if="isUserAdmin(user)"
										@click="promptConfirm(user.id, false)"
										class="bg-red text-white px-3 py-1 rounded-md hover:opacity-90 transition-colors font-medium cursor-pointer whitespace-nowrap">
										Премахни права
									</button>
								</div>
							</RouterLink>
						</td>
					</tr>
				</tbody>
			</table>
		</template>
	</div>
	<ConfirmationComponent
		v-if="showConfirmDialog"
		:message="confirmMessage"
		@confirm="handleConfirm(true)"
		@cancel="handleConfirm(false)"
		confirmText="Потвърди"
		cancelText="Откажи" />
</template>
