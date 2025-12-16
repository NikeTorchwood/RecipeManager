import { defineStore } from "pinia";
import {
  CategoriesClient,
  CategoryDto,
  CategoryDtoPageResult,
} from "@/api/webApiClient";

const API_URL = "http://localhost:5068";
const api = new CategoriesClient(API_URL);

interface CategoryFilterState {
  // возможно можно наследовать
  name?: string;
  pageSize: number;
  pageNumber: number;
  sortColumn?: string;
  sortDescending?: false;
}

export const useCategoryStore = defineStore("categories", {
  state: () => ({
    categories: [] as CategoryDto[],
    isLoading: false,
    filter: {
      name: undefined,
      pageSize: 10,
      pageNumber: 1,
      sortColumn: undefined,
      sortDescending: false,
    } as CategoryFilterState,
  }),

  actions: {
    async fetchCategoriesForDropdown() {
      this.isLoading = true;
      try {
        const f = this.filter;
        const result: CategoryDtoPageResult = await api.getAll(
          f.name,
          f.pageSize,
          f.pageNumber,
          f.sortColumn,
          f.sortDescending
        );

        this.categories = result.items || [];
      } catch (error) {
        console.error("Ошибка при загрузке категорий:", error);
      } finally {
        this.isLoading = false;
      }
    },
  },

  getters: {
    hasCategories: (state) => state.categories.length > 0,
  },
});
