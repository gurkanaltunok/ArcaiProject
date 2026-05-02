import React, { useState, useEffect } from 'react';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import apiService from '../services/apiService';
import { NotificationDto } from '../types';
import {
  Home,
  User,
  Users,
  Settings,
  Bell,
  LogOut,
  Menu,
  X,
  ClipboardList,
  BookOpen,
  History,
} from 'lucide-react';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Avatar, AvatarFallback } from './ui/avatar';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from './ui/dropdown-menu';
import { cn } from '../lib/utils';

const Layout: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, role, logout } = useAuth();
  const [unreadCount, setUnreadCount] = useState(0);
  const [notifications, setNotifications] = useState<NotificationDto[]>([]);
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  useEffect(() => {
    const fetchUnreadCount = async () => {
      try {
        const response = await apiService.get<number>('/notifications/my-unread-count');
        setUnreadCount(response.data);
      } catch (error) {
        console.error('Failed to fetch unread count:', error);
      }
    };

    fetchUnreadCount();
    const interval = setInterval(fetchUnreadCount, 30000);

    return () => clearInterval(interval);
  }, []);

  const handleNotificationsClick = async () => {
    try {
      const response = await apiService.get<NotificationDto[]>('/notifications/my-unread');
      setNotifications(response.data);
    } catch (error) {
      console.error('Failed to fetch notifications:', error);
    }
  };

  const handleNotificationClick = async (notification: NotificationDto) => {
    if (!notification.isRead) {
      try {
        await apiService.post(`/notifications/${notification.id}/mark-as-read`);
        setUnreadCount((prev) => Math.max(0, prev - 1));
      } catch (error) {
        console.error('Failed to mark notification as read:', error);
      }
    }

    if (notification.link) {
      navigate(notification.link);
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const getVisibleMenuItems = () => {
    const baseItems = [
      { text: 'Home', icon: Home, path: '/', roles: ['Admin', 'Professor'] },
    ];

    const professorItems = [
      { text: 'My Requests', icon: ClipboardList, path: '/my-requests', roles: ['Professor'] },
    ];

    const adminItems = [
      { text: 'Users', icon: Users, path: '/admin/users', roles: ['Admin'] },
      { text: 'Documents', icon: BookOpen, path: '/admin/documents', roles: ['Admin'] },
      { text: 'Requests', icon: ClipboardList, path: '/admin/requests', roles: ['Admin'] },
      { text: 'Borrowing History', icon: History, path: '/admin/borrowing-history', roles: ['Admin'] },
      { text: 'Settings', icon: Settings, path: '/admin/settings', roles: ['Admin'] },
    ];

    // Show items based on role
    if (role === 'Admin') {
      return [...baseItems, ...adminItems];
    } else if (role === 'Professor') {
      return [...baseItems, ...professorItems];
    }
    return baseItems;
  };

  const menuItems = getVisibleMenuItems();
  const isActive = (path: string) => {
    if (path === '/') {
      return location.pathname === '/';
    }
    return location.pathname.startsWith(path);
  };

  const canAccessPath = (itemPath: string) => {
    if (itemPath.startsWith('/admin')) {
      return role === 'Admin';
    }
    return true;
  };

  return (
    <div className="min-h-screen bg-background flex">
      {/* Sidebar */}
      <aside className={cn(
        "fixed left-0 top-0 z-40 h-screen w-64 border-r bg-card transition-transform",
        "lg:translate-x-0",
        mobileMenuOpen ? "translate-x-0" : "-translate-x-full lg:translate-x-0"
      )}>
        <div className="flex h-full flex-col">
          {/* Logo */}
          <div className="flex h-16 items-center justify-between border-b px-6">
            <div className="flex items-center gap-3">
              <img 
                src="/assets/images/gau-logo.png" 
                alt="University Logo" 
                className="h-14 w-auto object-contain"
                onError={(e) => {
                  e.currentTarget.style.display = 'none';
                }}
              />
              <h1 className="text-xl font-bold text-primary">ARCAI</h1>
            </div>
            <Button
              variant="ghost"
              size="icon"
              className="lg:hidden"
              onClick={() => setMobileMenuOpen(false)}
            >
              <X className="h-5 w-5" />
            </Button>
          </div>

          {/* Navigation */}
          <nav className="flex-1 space-y-1 px-3 py-4 overflow-y-auto">
            {menuItems.length === 0 ? (
              <div className="px-3 py-2 text-sm text-muted-foreground">
                Loading menu...
              </div>
            ) : (
              menuItems.map((item) => {
                const Icon = item.icon;
                const accessible = canAccessPath(item.path);
                return (
                  <button
                    key={item.path}
                    onClick={() => {
                      if (accessible) {
                        navigate(item.path);
                        setMobileMenuOpen(false);
                      }
                    }}
                    disabled={!accessible}
                    className={cn(
                      "flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors",
                      isActive(item.path) && accessible
                        ? "bg-primary text-primary-foreground shadow-sm"
                        : accessible
                        ? "text-muted-foreground hover:bg-accent hover:text-accent-foreground"
                        : "text-muted-foreground/50 cursor-not-allowed opacity-50"
                    )}
                  >
                    <Icon className="h-5 w-5 flex-shrink-0" />
                    <span>{item.text}</span>
                  </button>
                );
              })
            )}
          </nav>
        </div>
      </aside>

      {/* Mobile Menu Overlay */}
      {mobileMenuOpen && (
        <div
          className="fixed inset-0 z-30 bg-background/80 backdrop-blur-sm lg:hidden"
          onClick={() => setMobileMenuOpen(false)}
        />
      )}

      {/* Main Content */}
      <div className="flex-1 lg:pl-64">
        {/* Header */}
        <header className="sticky top-0 z-30 flex h-16 items-center gap-4 border-b bg-background px-4 sm:px-6">
          <Button
            variant="ghost"
            size="icon"
            className="lg:hidden"
            onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
          >
            {mobileMenuOpen ? (
              <X className="h-5 w-5" />
            ) : (
              <Menu className="h-5 w-5" />
            )}
          </Button>

          <div className="flex flex-1 items-center justify-end gap-2 sm:gap-4">
            {/* Notifications */}
            <DropdownMenu onOpenChange={(open) => {
              if (open) {
                handleNotificationsClick();
              }
            }}>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" size="icon" className="relative">
                  <Bell className="h-5 w-5" />
                  {unreadCount > 0 && (
                    <Badge
                      variant="destructive"
                      className="absolute -top-1 -right-1 h-5 w-5 rounded-full p-0 text-xs flex items-center justify-center"
                    >
                      {unreadCount > 9 ? '9+' : unreadCount}
                    </Badge>
                  )}
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-80">
                <div className="p-2">
                  <div className="mb-2 px-2 text-sm font-semibold">Notifications</div>
                  {notifications.length === 0 ? (
                    <div className="py-4 text-center text-sm text-muted-foreground">
                      No notifications
                    </div>
                  ) : (
                    <div className="space-y-1">
                      {notifications.map((notification) => (
                        <DropdownMenuItem
                          key={notification.id}
                          onClick={() => handleNotificationClick(notification)}
                          className={cn(
                            "cursor-pointer",
                            !notification.isRead && "bg-accent font-semibold"
                          )}
                        >
                          <div className="text-sm">{notification.message}</div>
                        </DropdownMenuItem>
                      ))}
                    </div>
                  )}
                </div>
              </DropdownMenuContent>
            </DropdownMenu>

            {/* User Menu */}
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" className="flex items-center gap-2">
                  <Avatar className="h-8 w-8">
                    <AvatarFallback className="text-xs">
                      {user?.firstName?.[0] || 'U'}{user?.lastName?.[0] || ''}
                    </AvatarFallback>
                  </Avatar>
                  <div className="hidden flex-col text-left text-sm lg:flex">
                    <span className="font-medium">
                      {user?.firstName || 'User'} {user?.lastName || ''}
                    </span>
                    <span className="text-xs text-muted-foreground">{role || 'User'}</span>
                  </div>
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-56">
                <div className="px-2 py-1.5">
                  <div className="text-sm font-medium">
                    {user?.firstName} {user?.lastName}
                  </div>
                  <div className="text-xs text-muted-foreground">{user?.email}</div>
                </div>
                <DropdownMenuSeparator />
                <DropdownMenuItem onClick={() => navigate('/profile')}>
                  <User className="mr-2 h-4 w-4" />
                  Profile
                </DropdownMenuItem>
                <DropdownMenuItem onClick={handleLogout}>
                  <LogOut className="mr-2 h-4 w-4" />
                  Logout
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        </header>

        {/* Page Content */}
        <main className="p-4 sm:p-6 lg:p-8 min-h-[calc(100vh-4rem)]">
          <Outlet />
        </main>
      </div>
    </div>
  );
};

export default Layout;
