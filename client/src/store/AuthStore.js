import { create } from "zustand";
import axiosInstance from "../Helper/AxioInstanse";
const useAuthStore = create(( set , get) => ({
    user: null,
    isSignUp: false,
    isLogin: false,
  
    signUp: async (data) => {
      set({ isSignUp: true });
      try {
        const response = await axiosInstance.post("/api/auth/signUp", data);
        return response.data;
      } catch (error) {
        console.error("Signup failed:", error?.response?.data || error.message);
      } finally {
        set({ isSignUp: false });
      }
    },
  
    login: async (data) => {
      set({ isLogin: true });
      try {
        const response = await axiosInstance.post("/api/auth/login", data);
        set({ user: response.data.data });
        return response.data ;
      } catch (error) {
        set({ user: null });
        console.error("Login failed:", error?.response?.data || error.message);
        throw error;
      } finally {
        set({ isLogin: false });
      }
    },

    googleLogin : async() => {
        set({ isLogin: true });
      try {
        // window.location.href = "http://localhost:5076/api/auth/google-login";

        // console.log(response)
        // set({ user: response.data.data });
        // return response.data ;
      } catch (error) {
        set({ user: null });
        console.error("Login failed:", error?.response?.data || error.message);
        throw error;
      } finally {
        set({ isLogin: false });
      }
    }
}));

  
  export {
    useAuthStore
  }