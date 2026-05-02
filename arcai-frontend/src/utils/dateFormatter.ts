// Date formatting utility for TRNC timezone (UTC+3, Europe/Nicosia)

const TIMEZONE = 'Europe/Nicosia'; // TRNC timezone (UTC+3)

/**
 * Formats a date string to TRNC timezone with 24-hour format
 * @param dateString - ISO date string from backend
 * @returns Formatted date and time string (DD.MM.YYYY HH:mm)
 */
export const formatDateTime = (dateString?: string): string => {
  if (!dateString) return '-';
  
  const date = new Date(dateString);
  
  return date.toLocaleString('en-GB', {
    timeZone: TIMEZONE,
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    hour12: false, // 24-hour format
  });
};

/**
 * Formats a date string to TRNC timezone (date only)
 * @param dateString - ISO date string from backend
 * @returns Formatted date string (DD.MM.YYYY)
 */
export const formatDate = (dateString?: string): string => {
  if (!dateString) return '-';
  
  const date = new Date(dateString);
  
  return date.toLocaleDateString('en-GB', {
    timeZone: TIMEZONE,
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  });
};

/**
 * Formats a date string to TRNC timezone (time only)
 * @param dateString - ISO date string from backend
 * @returns Formatted time string (HH:mm)
 */
export const formatTime = (dateString?: string): string => {
  if (!dateString) return '-';
  
  const date = new Date(dateString);
  
  return date.toLocaleTimeString('en-GB', {
    timeZone: TIMEZONE,
    hour: '2-digit',
    minute: '2-digit',
    hour12: false, // 24-hour format
  });
};

/**
 * Gets current date in TRNC timezone for input fields
 * @returns Date string in YYYY-MM-DD format
 */
export const getTodayInTRNC = (): string => {
  const now = new Date();
  const trncDate = new Date(now.toLocaleString('en-US', { timeZone: TIMEZONE }));
  return trncDate.toISOString().split('T')[0];
};

