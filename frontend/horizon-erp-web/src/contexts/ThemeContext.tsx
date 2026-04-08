'use client';

import { createContext, useContext, useState, type ReactNode } from 'react';

type ThemeTemplate = 'modern' | 'classic' | 'minimal';

interface ThemeContextType {
  template: ThemeTemplate;
  isDark: boolean;
  setTemplate: (template: ThemeTemplate) => void;
  toggleDark: () => void;
}

const ThemeContext = createContext<ThemeContextType | null>(null);

export function ThemeProvider({ children }: { children: ReactNode }) {
  const [template, setTemplate] = useState<ThemeTemplate>('modern');
  const [isDark, setIsDark] = useState(false);

  const toggleDark = () => {
    setIsDark(prev => !prev);
    document.documentElement.classList.toggle('dark');
  };

  return (
    <ThemeContext.Provider value={{ template, isDark, setTemplate, toggleDark }}>
      {children}
    </ThemeContext.Provider>
  );
}

export const useTheme = () => {
  const ctx = useContext(ThemeContext);
  if (!ctx) throw new Error('useTheme must be used within ThemeProvider');
  return ctx;
};
