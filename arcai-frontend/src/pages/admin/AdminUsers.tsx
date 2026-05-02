import React, { useState, useEffect } from 'react';
import apiService from '../../services/apiService';
import { User, CreateUserDto, PagingMetadata } from '../../types';
import { parsePaginationHeader } from '../../utils/parsePaginationHeader';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Button } from '../../components/ui/button';
import { Input } from '../../components/ui/input';
import { Label } from '../../components/ui/label';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '../../components/ui/dialog';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '../../components/ui/table';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '../../components/ui/dropdown-menu';
import { Plus, Loader2, Users, MoreVertical, Trash2 } from 'lucide-react';

const AdminUsers: React.FC = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const [paginationMetadata, setPaginationMetadata] = useState<PagingMetadata | null>(null);
  const [createDialogOpen, setCreateDialogOpen] = useState(false);
  const [createLoading, setCreateLoading] = useState(false);

  const [formData, setFormData] = useState<CreateUserDto>({
    email: '',
    firstName: '',
    lastName: '',
    password: '',
    role: 'Professor',
  });

  useEffect(() => {
    fetchUsers();
  }, [pageNumber]);

  const fetchUsers = async () => {
    setLoading(true);
    setError('');
    try {
      const response = await apiService.get<User[]>('/users', {
        params: { pageNumber, pageSize },
      });

      setUsers(response.data);
      
      const paginationHeader = response.headers['x-pagination'];
      if (paginationHeader) {
        const metadata = parsePaginationHeader(paginationHeader);
        setPaginationMetadata(metadata);
      }
    } catch (err: any) {
      setError('An error occurred while loading users');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateClick = () => {
    setFormData({
      email: '',
      firstName: '',
      lastName: '',
      password: '',
      role: 'Professor',
    });
    setCreateDialogOpen(true);
  };

  const handleCreateSubmit = async () => {
    setCreateLoading(true);
    try {
      await apiService.post('/users/create-professor', formData);
      alert('User created successfully!');
      setCreateDialogOpen(false);
      fetchUsers();
    } catch (err: any) {
      alert(err.response?.data?.message || 'An error occurred while creating the user');
    } finally {
      setCreateLoading(false);
    }
  };

  const handleDelete = async (user: User) => {
    if (!window.confirm(`Are you sure you want to delete user ${user.email}?`)) {
      return;
    }

    try {
      await apiService.delete(`/users/${user.id}`);
      alert('User deleted successfully!');
      fetchUsers();
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || err.response?.data?.details || 'An error occurred while deleting the user';
      alert(errorMessage);
      console.error('Delete user error:', err);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">User Management</h1>
          <p className="mt-2 text-muted-foreground">
            View all users in the system and add new users
          </p>
        </div>
        <Button onClick={handleCreateClick}>
          <Plus className="mr-2 h-4 w-4" />
          Add New Professor
        </Button>
      </div>

      {error && (
        <div className="rounded-lg bg-destructive/10 p-4 text-sm text-destructive">
          {error}
        </div>
      )}

      {loading ? (
        <div className="flex items-center justify-center py-12">
          <Loader2 className="h-8 w-8 animate-spin text-primary" />
        </div>
      ) : (
        <Card>
          <CardHeader>
            <CardTitle>Users</CardTitle>
          </CardHeader>
          <CardContent>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>ID</TableHead>
                  <TableHead>First Name</TableHead>
                  <TableHead>Last Name</TableHead>
                  <TableHead>Email</TableHead>
                  <TableHead>Role</TableHead>
                  <TableHead className="text-right">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {users.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={6} className="text-center py-8">
                      <Users className="mx-auto h-12 w-12 text-muted-foreground mb-4" />
                      <p className="text-muted-foreground">No users found</p>
                    </TableCell>
                  </TableRow>
                ) : (
                  users.map((user) => (
                    <TableRow key={user.id}>
                      <TableCell>{user.id}</TableCell>
                      <TableCell className="font-medium">{user.firstName}</TableCell>
                      <TableCell>{user.lastName}</TableCell>
                      <TableCell>{user.email}</TableCell>
                      <TableCell>
                        <span className="rounded-full bg-primary/10 px-2 py-1 text-xs font-medium text-primary">
                          {user.role}
                        </span>
                      </TableCell>
                      <TableCell className="text-right">
                        {user.role !== 'Admin' && (
                          <DropdownMenu>
                            <DropdownMenuTrigger asChild>
                              <Button variant="ghost" size="icon">
                                <MoreVertical className="h-4 w-4" />
                              </Button>
                            </DropdownMenuTrigger>
                            <DropdownMenuContent align="end">
                              <DropdownMenuItem
                                onClick={() => handleDelete(user)}
                                className="text-destructive"
                              >
                                <Trash2 className="mr-2 h-4 w-4" />
                                Delete
                              </DropdownMenuItem>
                            </DropdownMenuContent>
                          </DropdownMenu>
                        )}
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>

            {paginationMetadata && paginationMetadata.totalPages > 1 && (
              <div className="mt-4 flex items-center justify-center gap-2">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setPageNumber((p) => Math.max(1, p - 1))}
                  disabled={pageNumber === 1}
                >
                  Previous
                </Button>
                <span className="text-sm text-muted-foreground">
                  Page {pageNumber} / {paginationMetadata.totalPages}
                </span>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setPageNumber((p) => Math.min(paginationMetadata.totalPages, p + 1))}
                  disabled={pageNumber === paginationMetadata.totalPages}
                >
                  Next
                </Button>
              </div>
            )}
          </CardContent>
        </Card>
      )}

      {/* Create User Dialog */}
      <Dialog open={createDialogOpen} onOpenChange={setCreateDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Add New Professor</DialogTitle>
            <DialogDescription>
              Fill out the form below to add a new professor to the system
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <Label htmlFor="email">Email *</Label>
              <Input
                id="email"
                type="email"
                value={formData.email}
                onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="firstName">First Name *</Label>
              <Input
                id="firstName"
                value={formData.firstName}
                onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="lastName">Last Name *</Label>
              <Input
                id="lastName"
                value={formData.lastName}
                onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="password">Password *</Label>
              <Input
                id="password"
                type="password"
                value={formData.password}
                onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                required
              />
              <p className="text-xs text-muted-foreground">
                Minimum 8 characters, must contain uppercase, lowercase, number and special character
              </p>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setCreateDialogOpen(false)}>
              Cancel
            </Button>
            <Button
              onClick={handleCreateSubmit}
              disabled={createLoading || !formData.email || !formData.firstName || !formData.lastName || !formData.password}
            >
              {createLoading ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Creating...
                </>
              ) : (
                'Create'
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default AdminUsers;
