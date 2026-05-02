import React, { useState, useEffect } from 'react';
import apiService from '../../services/apiService';
import { DocumentTypeDto, LocationDto, CourseDto, TagDto, AcademicPeriodDto } from '../../types';
import { formatDate } from '../../utils/dateFormatter';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Button } from '../../components/ui/button';
import { Input } from '../../components/ui/input';
import { Label } from '../../components/ui/label';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '../../components/ui/tabs';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '../../components/ui/table';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '../../components/ui/dialog';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '../../components/ui/dropdown-menu';
import { Plus, Loader2, Edit2, Trash2, MoreVertical, FileText, MapPin, BookOpen, Tag, Calendar } from 'lucide-react';

type SettingsType = 'documentType' | 'location' | 'course' | 'tag' | 'period';

const AdminSettings: React.FC = () => {
  const [activeTab, setActiveTab] = useState('documentType');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [dialogType, setDialogType] = useState<SettingsType>('documentType');
  const [dialogMode, setDialogMode] = useState<'create' | 'edit'>('create');
  const [selectedItem, setSelectedItem] = useState<any>(null);

  // DocumentTypes
  const [documentTypes, setDocumentTypes] = useState<DocumentTypeDto[]>([]);
  const [docTypesLoading, setDocTypesLoading] = useState(false);

  // Locations
  const [locations, setLocations] = useState<LocationDto[]>([]);
  const [locationsLoading, setLocationsLoading] = useState(false);

  // Courses
  const [courses, setCourses] = useState<CourseDto[]>([]);
  const [coursesLoading, setCoursesLoading] = useState(false);

  // Tags
  const [tags, setTags] = useState<TagDto[]>([]);
  const [tagsLoading, setTagsLoading] = useState(false);

  // AcademicPeriods
  const [academicPeriods, setAcademicPeriods] = useState<AcademicPeriodDto[]>([]);
  const [periodsLoading, setPeriodsLoading] = useState(false);

  // Form data
  const [formData, setFormData] = useState<any>({});

  useEffect(() => {
    if (activeTab === 'documentType') fetchDocumentTypes();
    if (activeTab === 'location') fetchLocations();
    if (activeTab === 'course') fetchCourses();
    if (activeTab === 'tag') fetchTags();
    if (activeTab === 'period') fetchAcademicPeriods();
  }, [activeTab]);

  const fetchDocumentTypes = async () => {
    setDocTypesLoading(true);
    try {
      const response = await apiService.get<DocumentTypeDto[]>('/documenttypes', {
        params: { pageNumber: 1, pageSize: 100 },
      });
      setDocumentTypes(response.data);
    } catch (err) {
      console.error('Error loading DocumentTypes:', err);
    } finally {
      setDocTypesLoading(false);
    }
  };

  const fetchLocations = async () => {
    setLocationsLoading(true);
    try {
      const response = await apiService.get<LocationDto[]>('/locations', {
        params: { pageNumber: 1, pageSize: 100 },
      });
      setLocations(response.data);
    } catch (err) {
      console.error('Error loading Locations:', err);
    } finally {
      setLocationsLoading(false);
    }
  };

  const fetchCourses = async () => {
    setCoursesLoading(true);
    try {
      const response = await apiService.get<CourseDto[]>('/courses', {
        params: { pageNumber: 1, pageSize: 100 },
      });
      setCourses(response.data);
    } catch (err) {
      console.error('Error loading Courses:', err);
    } finally {
      setCoursesLoading(false);
    }
  };

  const fetchTags = async () => {
    setTagsLoading(true);
    try {
      const response = await apiService.get<TagDto[]>('/tags', {
        params: { pageNumber: 1, pageSize: 100 },
      });
      setTags(response.data);
    } catch (err) {
      console.error('Error loading Tags:', err);
    } finally {
      setTagsLoading(false);
    }
  };

  const fetchAcademicPeriods = async () => {
    setPeriodsLoading(true);
    try {
      const response = await apiService.get<AcademicPeriodDto[]>('/academicperiods', {
        params: { pageNumber: 1, pageSize: 100 },
      });
      setAcademicPeriods(response.data);
    } catch (err) {
      console.error('Error loading AcademicPeriods:', err);
    } finally {
      setPeriodsLoading(false);
    }
  };

  const handleCreate = (type: SettingsType) => {
    setDialogType(type);
    setDialogMode('create');
    setSelectedItem(null);
    setFormData({});
    setDialogOpen(true);
  };

  const handleEdit = (item: any, type: SettingsType) => {
    setDialogType(type);
    setDialogMode('edit');
    setSelectedItem(item);
    setFormData(item);
    setDialogOpen(true);
  };

  const handleDelete = async (id: number, type: SettingsType) => {
    if (!window.confirm('Are you sure you want to delete this item?')) {
      return;
    }

    try {
      const endpoints: Record<SettingsType, string> = {
        documentType: '/documenttypes',
        location: '/locations',
        course: '/courses',
        tag: '/tags',
        period: '/academicperiods',
      };

      await apiService.delete(`${endpoints[type]}/${id}`);
      alert('Item deleted successfully!');

      if (type === 'documentType') fetchDocumentTypes();
      if (type === 'location') fetchLocations();
      if (type === 'course') fetchCourses();
      if (type === 'tag') fetchTags();
      if (type === 'period') fetchAcademicPeriods();
    } catch (err: any) {
      alert(err.response?.data?.message || 'An error occurred while deleting the item');
    }
  };

  const handleDialogSubmit = async () => {
    try {
      const endpoints: Record<SettingsType, string> = {
        documentType: '/documenttypes',
        location: '/locations',
        course: '/courses',
        tag: '/tags',
        period: '/academicperiods',
      };

      if (dialogMode === 'create') {
        await apiService.post(endpoints[dialogType], formData);
        alert('Item created successfully!');
      } else {
        await apiService.put(`${endpoints[dialogType]}/${selectedItem.id}`, formData);
        alert('Item updated successfully!');
      }

      setDialogOpen(false);
      
      if (dialogType === 'documentType') fetchDocumentTypes();
      if (dialogType === 'location') fetchLocations();
      if (dialogType === 'course') fetchCourses();
      if (dialogType === 'tag') fetchTags();
      if (dialogType === 'period') fetchAcademicPeriods();
    } catch (err: any) {
      alert(err.response?.data?.message || 'An error occurred during the operation');
    }
  };


  const getDialogTitle = () => {
    const titles: Record<SettingsType, string> = {
      documentType: 'Document Type',
      location: 'Location',
      course: 'Course',
      tag: 'Tag',
      period: 'Academic Period',
    };
    return `${dialogMode === 'create' ? 'New' : 'Edit'} ${titles[dialogType]}`;
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Settings</h1>
        <p className="mt-2 text-muted-foreground">
          Manage system settings and lookup tables
        </p>
      </div>

      <Tabs value={activeTab} onValueChange={setActiveTab}>
        <TabsList className="grid w-full grid-cols-5">
          <TabsTrigger value="documentType">
            <FileText className="mr-2 h-4 w-4" />
            Document Types
          </TabsTrigger>
          <TabsTrigger value="location">
            <MapPin className="mr-2 h-4 w-4" />
            Locations
          </TabsTrigger>
          <TabsTrigger value="course">
            <BookOpen className="mr-2 h-4 w-4" />
            Courses
          </TabsTrigger>
          <TabsTrigger value="tag">
            <Tag className="mr-2 h-4 w-4" />
            Tags
          </TabsTrigger>
          <TabsTrigger value="period">
            <Calendar className="mr-2 h-4 w-4" />
            Periods
          </TabsTrigger>
        </TabsList>

        <TabsContent value="documentType" className="space-y-4">
          <div className="flex justify-end">
            <Button onClick={() => handleCreate('documentType')}>
              <Plus className="mr-2 h-4 w-4" />
              Add New Document Type
            </Button>
          </div>
          <Card>
            <CardHeader>
              <CardTitle>Document Types</CardTitle>
            </CardHeader>
            <CardContent>
              {docTypesLoading ? (
                <div className="flex items-center justify-center py-12">
                  <Loader2 className="h-8 w-8 animate-spin text-primary" />
                </div>
              ) : (
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>ID</TableHead>
                      <TableHead>Name</TableHead>
                      <TableHead>Description</TableHead>
                      <TableHead className="text-right">Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {documentTypes.length === 0 ? (
                      <TableRow>
                        <TableCell colSpan={4} className="text-center py-8">
                          No document types found
                        </TableCell>
                      </TableRow>
                    ) : (
                      documentTypes.map((dt) => (
                        <TableRow key={dt.id}>
                          <TableCell>{dt.id}</TableCell>
                          <TableCell className="font-medium">{dt.name}</TableCell>
                          <TableCell>{dt.description || '-'}</TableCell>
                          <TableCell className="text-right">
                            <DropdownMenu>
                              <DropdownMenuTrigger asChild>
                                <Button variant="ghost" size="icon">
                                  <MoreVertical className="h-4 w-4" />
                                </Button>
                              </DropdownMenuTrigger>
                              <DropdownMenuContent align="end">
                                <DropdownMenuItem onClick={() => handleEdit(dt, 'documentType')}>
                                  <Edit2 className="mr-2 h-4 w-4" />
                                  Edit
                                </DropdownMenuItem>
                                <DropdownMenuItem
                                  onClick={() => handleDelete(dt.id, 'documentType')}
                                  className="text-destructive"
                                >
                                  <Trash2 className="mr-2 h-4 w-4" />
                                  Delete
                                </DropdownMenuItem>
                              </DropdownMenuContent>
                            </DropdownMenu>
                          </TableCell>
                        </TableRow>
                      ))
                    )}
                  </TableBody>
                </Table>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="location" className="space-y-4">
          <div className="flex justify-end">
            <Button onClick={() => handleCreate('location')}>
              <Plus className="mr-2 h-4 w-4" />
              Add New Location
            </Button>
          </div>
          <Card>
            <CardHeader>
              <CardTitle>Locations</CardTitle>
            </CardHeader>
            <CardContent>
              {locationsLoading ? (
                <div className="flex items-center justify-center py-12">
                  <Loader2 className="h-8 w-8 animate-spin text-primary" />
                </div>
              ) : (
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>ID</TableHead>
                      <TableHead>Room</TableHead>
                      <TableHead>Cabinet</TableHead>
                      <TableHead>Shelf</TableHead>
                      <TableHead className="text-right">Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {locations.length === 0 ? (
                      <TableRow>
                        <TableCell colSpan={5} className="text-center py-8">
                          No locations found
                        </TableCell>
                      </TableRow>
                    ) : (
                      locations.map((loc) => (
                        <TableRow key={loc.id}>
                          <TableCell>{loc.id}</TableCell>
                          <TableCell className="font-medium">{loc.room}</TableCell>
                          <TableCell>{loc.cabinet || '-'}</TableCell>
                          <TableCell>{loc.shelf || '-'}</TableCell>
                          <TableCell className="text-right">
                            <DropdownMenu>
                              <DropdownMenuTrigger asChild>
                                <Button variant="ghost" size="icon">
                                  <MoreVertical className="h-4 w-4" />
                                </Button>
                              </DropdownMenuTrigger>
                              <DropdownMenuContent align="end">
                                <DropdownMenuItem onClick={() => handleEdit(loc, 'location')}>
                                  <Edit2 className="mr-2 h-4 w-4" />
                                  Edit
                                </DropdownMenuItem>
                                <DropdownMenuItem
                                  onClick={() => handleDelete(loc.id, 'location')}
                                  className="text-destructive"
                                >
                                  <Trash2 className="mr-2 h-4 w-4" />
                                  Delete
                                </DropdownMenuItem>
                              </DropdownMenuContent>
                            </DropdownMenu>
                          </TableCell>
                        </TableRow>
                      ))
                    )}
                  </TableBody>
                </Table>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="course" className="space-y-4">
          <div className="flex justify-end">
            <Button onClick={() => handleCreate('course')}>
              <Plus className="mr-2 h-4 w-4" />
              Add New Course
            </Button>
          </div>
          <Card>
            <CardHeader>
              <CardTitle>Courses</CardTitle>
            </CardHeader>
            <CardContent>
              {coursesLoading ? (
                <div className="flex items-center justify-center py-12">
                  <Loader2 className="h-8 w-8 animate-spin text-primary" />
                </div>
              ) : (
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>ID</TableHead>
                      <TableHead>Course Code</TableHead>
                      <TableHead>Course Name</TableHead>
                      <TableHead>Department</TableHead>
                      <TableHead className="text-right">Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {courses.length === 0 ? (
                      <TableRow>
                        <TableCell colSpan={5} className="text-center py-8">
                          No courses found
                        </TableCell>
                      </TableRow>
                    ) : (
                      courses.map((course) => (
                        <TableRow key={course.id}>
                          <TableCell>{course.id}</TableCell>
                          <TableCell className="font-medium">{course.courseCode}</TableCell>
                          <TableCell>{course.courseName}</TableCell>
                          <TableCell>{course.department || '-'}</TableCell>
                          <TableCell className="text-right">
                            <DropdownMenu>
                              <DropdownMenuTrigger asChild>
                                <Button variant="ghost" size="icon">
                                  <MoreVertical className="h-4 w-4" />
                                </Button>
                              </DropdownMenuTrigger>
                              <DropdownMenuContent align="end">
                                <DropdownMenuItem onClick={() => handleEdit(course, 'course')}>
                                  <Edit2 className="mr-2 h-4 w-4" />
                                  Edit
                                </DropdownMenuItem>
                                <DropdownMenuItem
                                  onClick={() => handleDelete(course.id, 'course')}
                                  className="text-destructive"
                                >
                                  <Trash2 className="mr-2 h-4 w-4" />
                                  Delete
                                </DropdownMenuItem>
                              </DropdownMenuContent>
                            </DropdownMenu>
                          </TableCell>
                        </TableRow>
                      ))
                    )}
                  </TableBody>
                </Table>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="tag" className="space-y-4">
          <div className="flex justify-end">
            <Button onClick={() => handleCreate('tag')}>
              <Plus className="mr-2 h-4 w-4" />
              Add New Tag
            </Button>
          </div>
          <Card>
            <CardHeader>
              <CardTitle>Tags</CardTitle>
            </CardHeader>
            <CardContent>
              {tagsLoading ? (
                <div className="flex items-center justify-center py-12">
                  <Loader2 className="h-8 w-8 animate-spin text-primary" />
                </div>
              ) : (
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>ID</TableHead>
                      <TableHead>Name</TableHead>
                      <TableHead className="text-right">Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {tags.length === 0 ? (
                      <TableRow>
                        <TableCell colSpan={3} className="text-center py-8">
                          No tags found
                        </TableCell>
                      </TableRow>
                    ) : (
                      tags.map((tag) => (
                        <TableRow key={tag.id}>
                          <TableCell>{tag.id}</TableCell>
                          <TableCell className="font-medium">{tag.name}</TableCell>
                          <TableCell className="text-right">
                            <DropdownMenu>
                              <DropdownMenuTrigger asChild>
                                <Button variant="ghost" size="icon">
                                  <MoreVertical className="h-4 w-4" />
                                </Button>
                              </DropdownMenuTrigger>
                              <DropdownMenuContent align="end">
                                <DropdownMenuItem onClick={() => handleEdit(tag, 'tag')}>
                                  <Edit2 className="mr-2 h-4 w-4" />
                                  Edit
                                </DropdownMenuItem>
                                <DropdownMenuItem
                                  onClick={() => handleDelete(tag.id, 'tag')}
                                  className="text-destructive"
                                >
                                  <Trash2 className="mr-2 h-4 w-4" />
                                  Delete
                                </DropdownMenuItem>
                              </DropdownMenuContent>
                            </DropdownMenu>
                          </TableCell>
                        </TableRow>
                      ))
                    )}
                  </TableBody>
                </Table>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="period" className="space-y-4">
          <div className="flex justify-end">
            <Button onClick={() => handleCreate('period')}>
              <Plus className="mr-2 h-4 w-4" />
              Add New Academic Period
            </Button>
          </div>
          <Card>
            <CardHeader>
              <CardTitle>Academic Periods</CardTitle>
            </CardHeader>
            <CardContent>
              {periodsLoading ? (
                <div className="flex items-center justify-center py-12">
                  <Loader2 className="h-8 w-8 animate-spin text-primary" />
                </div>
              ) : (
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>ID</TableHead>
                      <TableHead>Period Name</TableHead>
                      <TableHead>Start Date</TableHead>
                      <TableHead>End Date</TableHead>
                      <TableHead className="text-right">Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {academicPeriods.length === 0 ? (
                      <TableRow>
                        <TableCell colSpan={5} className="text-center py-8">
                          No academic periods found
                        </TableCell>
                      </TableRow>
                    ) : (
                      academicPeriods.map((period) => (
                        <TableRow key={period.id}>
                          <TableCell>{period.id}</TableCell>
                          <TableCell className="font-medium">{period.periodName}</TableCell>
                          <TableCell>{formatDate(period.startDate)}</TableCell>
                          <TableCell>{formatDate(period.endDate)}</TableCell>
                          <TableCell className="text-right">
                            <DropdownMenu>
                              <DropdownMenuTrigger asChild>
                                <Button variant="ghost" size="icon">
                                  <MoreVertical className="h-4 w-4" />
                                </Button>
                              </DropdownMenuTrigger>
                              <DropdownMenuContent align="end">
                                <DropdownMenuItem onClick={() => handleEdit(period, 'period')}>
                                  <Edit2 className="mr-2 h-4 w-4" />
                                  Edit
                                </DropdownMenuItem>
                                <DropdownMenuItem
                                  onClick={() => handleDelete(period.id, 'period')}
                                  className="text-destructive"
                                >
                                  <Trash2 className="mr-2 h-4 w-4" />
                                  Delete
                                </DropdownMenuItem>
                              </DropdownMenuContent>
                            </DropdownMenu>
                          </TableCell>
                        </TableRow>
                      ))
                    )}
                  </TableBody>
                </Table>
              )}
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>

      {/* Generic CRUD Dialog */}
      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent className="max-w-md">
          <DialogHeader>
            <DialogTitle>{getDialogTitle()}</DialogTitle>
            <DialogDescription>
              {dialogMode === 'create' ? 'Create a new item' : 'Edit the item'}
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4 py-4">
            {dialogType === 'documentType' && (
              <>
                <div className="space-y-2">
                  <Label htmlFor="name">Name *</Label>
                  <Input
                    id="name"
                    value={formData.name || ''}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="description">Description</Label>
                  <Input
                    id="description"
                    value={formData.description || ''}
                    onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                  />
                </div>
              </>
            )}
            {dialogType === 'location' && (
              <>
                <div className="space-y-2">
                  <Label htmlFor="room">Room *</Label>
                  <Input
                    id="room"
                    value={formData.room || ''}
                    onChange={(e) => setFormData({ ...formData, room: e.target.value })}
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="cabinet">Cabinet</Label>
                  <Input
                    id="cabinet"
                    value={formData.cabinet || ''}
                    onChange={(e) => setFormData({ ...formData, cabinet: e.target.value })}
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="shelf">Shelf</Label>
                  <Input
                    id="shelf"
                    value={formData.shelf || ''}
                    onChange={(e) => setFormData({ ...formData, shelf: e.target.value })}
                  />
                </div>
              </>
            )}
            {dialogType === 'course' && (
              <>
                <div className="space-y-2">
                  <Label htmlFor="courseCode">Course Code *</Label>
                  <Input
                    id="courseCode"
                    value={formData.courseCode || ''}
                    onChange={(e) => setFormData({ ...formData, courseCode: e.target.value })}
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="courseName">Course Name *</Label>
                  <Input
                    id="courseName"
                    value={formData.courseName || ''}
                    onChange={(e) => setFormData({ ...formData, courseName: e.target.value })}
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="department">Department</Label>
                  <Input
                    id="department"
                    value={formData.department || ''}
                    onChange={(e) => setFormData({ ...formData, department: e.target.value })}
                  />
                </div>
              </>
            )}
            {dialogType === 'tag' && (
              <div className="space-y-2">
                <Label htmlFor="name">Name *</Label>
                <Input
                  id="name"
                  value={formData.name || ''}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                />
              </div>
            )}
            {dialogType === 'period' && (
              <>
                <div className="space-y-2">
                  <Label htmlFor="periodName">Period Name *</Label>
                  <Input
                    id="periodName"
                    value={formData.periodName || ''}
                    onChange={(e) => setFormData({ ...formData, periodName: e.target.value })}
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="startDate">Start Date</Label>
                  <Input
                    id="startDate"
                    type="date"
                    value={formData.startDate ? new Date(formData.startDate).toISOString().split('T')[0] : ''}
                    onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="endDate">End Date</Label>
                  <Input
                    id="endDate"
                    type="date"
                    value={formData.endDate ? new Date(formData.endDate).toISOString().split('T')[0] : ''}
                    onChange={(e) => setFormData({ ...formData, endDate: e.target.value })}
                  />
                </div>
              </>
            )}
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setDialogOpen(false)}>
              Cancel
            </Button>
            <Button onClick={handleDialogSubmit}>
              {dialogMode === 'create' ? 'Create' : 'Update'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default AdminSettings;
