import { defineStore } from "pinia";
import {
  RecipesClient,
  RecipeShortDto,
  RecipeCreateDto,
  RecipeShortDtoPageResult,
} from "@/api/webApiClient";

const API_URL = "http://localhost:5068";
const api = new RecipesClient(API_URL);

interface RecipeFilterState {
  name?: string;
  description?: string;
  categoryId?: string;
  ingredientId?: string;
  createdFrom?: Date;
  createdTo?: Date;
  pageSize: number;
  pageNumber: number;
  sortColumn?: string;
  sortDescending?: boolean;
}

export const useRecipeStore = defineStore("recipes", {
  state: () => ({
    recipes: [] as RecipeShortDto[], //пока как заглушка
    totalCount: 0,

    filter: {
      name: undefined,
      description: undefined,
      categoryId: undefined,
      ingredientId: undefined,
      createdFrom: undefined,
      createdTo: undefined,
      pageSize: 10,
      pageNumber: 1,
      sortColumn: undefined,
      sortDescending: false,
    } as RecipeFilterState,

    isLoading: false,
  }),

  actions: {
    async fetchRecipes() {
      this.isLoading = true;
      try {
        const f = this.filter;

        const result: RecipeShortDtoPageResult = await api.getAll(
          f.name,
          f.description,
          f.categoryId,
          f.ingredientId,
          f.createdFrom,
          f.createdTo,
          f.pageSize,
          f.pageNumber,
          f.sortColumn,
          f.sortDescending
        );

        this.recipes = result.items || [];
        this.totalCount = result.totalCount || 0;
      } catch (error) {
        console.error("Ошибка при получении рецептов:", error);
      } finally {
        this.isLoading = false;
      }
    },

    async setPage(page: number) {
      this.filter.pageNumber = page;
      await this.fetchRecipes();
    },

    async searchByName(name: string) {
      this.filter.name = name || undefined;
      this.filter.pageNumber = 1;
      await this.fetchRecipes();
    },

    async filterByCategory(categoryId: string | undefined) {
      this.filter.categoryId = categoryId;
      this.filter.pageNumber = 1;
      await this.fetchRecipes();
    },

    async addRecipe(recipe: RecipeCreateDto) {
      await api.create(recipe);
      await this.fetchRecipes();
    },

    async updateRecipe(id: string, recipe: RecipeCreateDto) {
      await api.update(id, recipe);
      await this.fetchRecipes();
    },

    async deleteRecipe(id: string) {
      await api.delete(id);
      this.recipes = this.recipes.filter((r) => r.id !== id);
      this.totalCount--;

      if (this.recipes.length === 0 && this.filter.pageNumber > 1) {
        await this.setPage(this.filter.pageNumber - 1);
      } else {
        await this.fetchRecipes();
      }
    },
  },

  getters: {
    totalPages: (state) => Math.ceil(state.totalCount / state.filter.pageSize),
  },
});
