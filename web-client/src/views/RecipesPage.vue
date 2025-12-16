<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import { storeToRefs } from "pinia";
import { useRecipeStore } from '../stores/recipes.store';
import { useCategoryStore } from "@/stores/category.store";
import { RecipeCreateDto, RecipeShortDto } from "@/api/webApiClient";
import type { Recipe } from '../models/Recipe';

import RecipeList from '../components/RecipeList.vue';
import DefaultLayout from '../layouts/DefaultLayout.vue';
import PageHeaderRecipes from '../components/PageHeaderRecipes.vue';
import AddOrUpdateRecipeDialog from '../components/AddOrUpdateRecipeDialog.vue';

const recipeStore = useRecipeStore();
const { recipes, totalCount, isLoading } = storeToRefs(recipeStore);

const categoryStore = useCategoryStore();
const { categories } = storeToRefs(categoryStore);

const isDialogOpen = ref(false);
const editingRecipe = ref<RecipeShortDto | null>(null);

onMounted(() => {
    recipeStore.fetchRecipes();
    categoryStore.fetchCategoriesForDropdown();
});

const mappedRecipes = computed<Recipe[]>(() => {
    return recipes.value.map(dto => ({
        id: dto.id || "",
        title: dto.name || "Без названия",
        description: dto.description || "",
        categoryId: dto.categoryId
    }));
});

const recipeForDialog = computed<Recipe | undefined>(() => {
    if (!editingRecipe.value) return undefined;

    return {
        id: editingRecipe.value.id || "",
        title: editingRecipe.value.name || "",
        description: editingRecipe.value.description || "",
        categoryId: editingRecipe.value.categoryId
    };
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
    if (!formResult.categoryId) {
        console.error("Нет категории");
        return;
    }

    const dto = new RecipeCreateDto({
        name: formResult.title,
        description: formResult.description,
        categoryId: formResult.categoryId,
        ingredients: []
    });

    try {
        if (editingRecipe.value && editingRecipe.value.id) {
            await recipeStore.updateRecipe(editingRecipe.value.id, dto);
        } else {
            await recipeStore.addRecipe(dto);
        }
        isDialogOpen.value = false;
    } catch (e) {
        alert("Ошибка при сохранении");
    }
};

const handleDeleteRecipe = async (id: string) => {
    if (!confirm("Вы уверены?")) return;
    await recipeStore.deleteRecipe(id);
};
</script>

<template>
    <DefaultLayout>
        <template #header>
            <PageHeaderRecipes :recipe-count="totalCount" @add-clicked="openAddDialog" />
        </template>

        <template #content>
            <div v-if="isLoading">Загрузка...</div>

            <AddOrUpdateRecipeDialog v-model:visible="isDialogOpen" :categories="categories" :recipe="recipeForDialog"
                @submit="handleSubmitRecipe" />

            <RecipeList :recipe-list="mappedRecipes" @edit="openEditDialog" @delete="handleDeleteRecipe" />
        </template>
    </DefaultLayout>
</template>