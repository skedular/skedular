export const isWeeklySelectionComplete = <T>(selectionRequired: boolean, selectedDays: readonly T[], requiredDaysPerWeek: number) =>
  !selectionRequired || selectedDays.length === requiredDaysPerWeek;

export const toggleWeeklySelectedDay = <T>(selectedDays: readonly T[], day: T, requiredDaysPerWeek: number): T[] => {
  if (selectedDays.includes(day)) {
    return selectedDays.filter((item) => item !== day);
  }

  return selectedDays.length >= requiredDaysPerWeek ? [...selectedDays] : [...selectedDays, day];
};

export const toWeeklySelectedDaysInput = <T>(selectionRequired: boolean, selectedDays: readonly T[]): T[] => (selectionRequired ? [...selectedDays] : []);
