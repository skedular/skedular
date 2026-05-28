'use client';

import { useEffect, useState } from 'react';

const leftNavigationCollapsedStorageKey = 'skedular:left-navigation-collapsed';

const readCollapsedPreference = (): boolean | null => {
  if (typeof window === 'undefined') {
    return null;
  }

  const value = window.localStorage.getItem(leftNavigationCollapsedStorageKey);

  if (value === null) {
    return null;
  }

  return value === 'true';
};

const useLeftNavigationCollapsed = () => {
  const [isCollapsed, setIsCollapsed] = useState(false);
  const [isReadyToPersist, setIsReadyToPersist] = useState(false);

  useEffect(() => {
    const storedPreference = readCollapsedPreference();

    if (storedPreference !== null) {
      setIsCollapsed(storedPreference);
    }

    setIsReadyToPersist(true);
  }, []);

  useEffect(() => {
    if (!isReadyToPersist || typeof window === 'undefined') {
      return;
    }

    window.localStorage.setItem(leftNavigationCollapsedStorageKey, isCollapsed ? 'true' : 'false');
  }, [isCollapsed, isReadyToPersist]);

  return [isCollapsed, setIsCollapsed] as const;
};

export default useLeftNavigationCollapsed;
