<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import { storeToRefs } from "pinia";
import { useRecipeStore } from '../stores/recipes.store';
import { RecipeCreateDto, RecipeShortDto } from "@/api/webApiClient";
import type { Recipe } from '../models/Recipe';


import RecipeList from '../components/RecipeList.vue';
import DefaultLayout from '../layouts/DefaultLayout.vue';
import PageHeaderRecipes from '../components/PageHeaderRecipes.vue';
import AddOrUpdateRecipeDialog from '../components/AddOrUpdateRecipeDialog.vue';


const store = useRecipeStore();
const { recipes, totalCount, isLoading } = storeToRefs(store);

const isDialogOpen = ref(false);
const editingRecipe = ref<RecipeShortDto | null>(null);

onMounted(() => {
    store.fetchRecipes();
});

const mappedRecipes = computed<Recipe[]>(() => {
    return recipes.value.map(dto => ({
        id: dto.id || "",
        title: dto.name || "Без названия",
        description: dto.description || ""
    }));
});

const openAddDialog = () => {
    editingRecipe.value = null;
    isDialogOpen.value = true;
};

const openEditDialog = (recipe: Recipe) => {
    const found = recipes.value.find(r => r.id === recipe.id);
    if (found) {
        editingRecipe.value = found;
        isDialogOpen.value = true;
    }
};

const handleSubmitRecipe = async (formResult: Recipe) => {
    const dto = new RecipeCreateDto({
        name: formResult.title,
        description: formResult.description,
        categoryId: crypto.randomUUID(),
        ingredients: []
    });

    if (editingRecipe.value && editingRecipe.value.id) {
        await store.updateRecipe(editingRecipe.value.id, dto);
    } else {
        await store.addRecipe(dto);
    }

    isDialogOpen.value = false;
};

const handleDeleteRecipe = async (id: string) => {
    if (!confirm("Вы уверены?")) return;
    await store.deleteRecipe(id);
};
</script>

<template>
    <DefaultLayout>
        <template #header>
            <PageHeaderRecipes :recipe-count="totalCount" @add-clicked="openAddDialog" />
        </template>

        <template #content>
            <div v-if="isLoading">Загрузка...</div>

            <AddOrUpdateRecipeDialog v-model:visible="isDialogOpen"
                :recipe="editingRecipe ? { id: editingRecipe.id!, title: editingRecipe.name!, description: editingRecipe.description! } : undefined"
                @submit="handleSubmitRecipe" />

            <RecipeList :recipe-list="mappedRecipes" @edit="openEditDialog" @delete="handleDeleteRecipe" />

        </template>
    </DefaultLayout>
</template>