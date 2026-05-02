import React, { useState, useEffect } from 'react';
import apiService from '../../services/apiService';
import { DocumentDto, CreateDocumentDto, DocumentTypeDto, LocationDto, CourseDto, AcademicPeriodDto, TagDto, PagingMetadata } from '../../types';
import { parsePaginationHeader } from '../../utils/parsePaginationHeader';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Button } from '../../components/ui/button';
import { Badge } from '../../components/ui/badge';
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
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../../components/ui/select';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '../../components/ui/table';
import { Plus, Loader2, FileText, Tag as TagIcon, X } from 'lucide-react';
import { cn } from '../../lib/utils';

const AdminDocuments: React.FC = () => {
  const [documents, setDocuments] = useState<DocumentDto[]>([]);
  const [documentTypes, setDocumentTypes] = useState<DocumentTypeDto[]>([]);
  const [locations, setLocations] = useState<LocationDto[]>([]);
  const [courses, setCourses] = useState<CourseDto[]>([]);
  const [academicPeriods, setAcademicPeriods] = useState<AcademicPeriodDto[]>([]);
  const [tags, setTags] = useState<TagDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const [paginationMetadata, setPaginationMetadata] = useState<PagingMetadata | null>(null);
  const [createDialogOpen, setCreateDialogOpen] = useState(false);
  const [createLoading, setCreateLoading] = useState(false);
  
  // Filter states
  const [filterDocumentTypeId, setFilterDocumentTypeId] = useState<number | undefined>(undefined);
  const [filterAcademicPeriodId, setFilterAcademicPeriodId] = useState<number | undefined>(undefined);
  const [filterLocationId, setFilterLocationId] = useState<number | undefined>(undefined);
  const [filterCourseId, setFilterCourseId] = useState<number | undefined>(undefined);
  const [filterStatus, setFilterStatus] = useState<string | undefined>(undefined);
  const [filterTagId, setFilterTagId] = useState<number | undefined>(undefined);
  const [searchTitle, setSearchTitle] = useState<string>('');
  const [searchTitleInput, setSearchTitleInput] = useState<string>('');

  const [formData, setFormData] = useState<CreateDocumentDto>({
    title: '',
    description: '',
    documentTypeId: 0,
    locationId: 0,
    courseId: undefined,
    academicPeriodId: undefined,
    tagIds: [],
  });

  useEffect(() => {
    fetchDocuments();
  }, [pageNumber, filterDocumentTypeId, filterAcademicPeriodId, filterLocationId, filterCourseId, filterStatus, filterTagId, searchTitle]);

  // Debounce search title
  useEffect(() => {
    const timer = setTimeout(() => {
      setSearchTitle(searchTitleInput);
      setPageNumber(1); // Reset to first page when search changes
    }, 500); // 500ms delay

    return () => clearTimeout(timer);
  }, [searchTitleInput]);

  useEffect(() => {
    fetchLookupData();
  }, []);

  const fetchDocuments = async () => {
    setLoading(true);
    setError('');
    try {
      const params: any = { PageNumber: pageNumber, PageSize: pageSize };
      
      // Add filter parameters if they are set (using PascalCase to match backend properties)
      if (filterDocumentTypeId && filterDocumentTypeId > 0) {
        params.DocumentTypeId = filterDocumentTypeId;
      }
      if (filterAcademicPeriodId && filterAcademicPeriodId > 0) {
        params.AcademicPeriodId = filterAcademicPeriodId;
      }
      if (filterLocationId && filterLocationId > 0) {
        params.LocationId = filterLocationId;
      }
      if (filterCourseId && filterCourseId > 0) {
        params.CourseId = filterCourseId;
      }
      if (filterStatus && filterStatus !== '') {
        params.Status = filterStatus;
      }
      if (filterTagId && filterTagId > 0) {
        params.TagId = filterTagId;
      }
      if (searchTitle && searchTitle.trim() !== '') {
        params.SearchTitle = searchTitle.trim();
      }

      console.log('Sending params:', params); // Debug log

      const response = await apiService.get<DocumentDto[]>('/documents', {
        params,
      });


      setDocuments(response.data);
      
      const paginationHeader = response.headers['x-pagination'];
      if (paginationHeader) {
        const metadata = parsePaginationHeader(paginationHeader);
        setPaginationMetadata(metadata);
      }
    } catch (err: any) {
      setError('An error occurred while loading documents');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };
  
  const handleFilterChange = () => {
    // Reset to first page when filters change
    setPageNumber(1);
  };
  
  const handleClearFilters = () => {
    setFilterDocumentTypeId(undefined);
    setFilterAcademicPeriodId(undefined);
    setFilterLocationId(undefined);
    setFilterCourseId(undefined);
    setFilterStatus(undefined);
    setFilterTagId(undefined);
    setSearchTitle('');
    setSearchTitleInput('');
    setPageNumber(1);
  };

  const fetchLookupData = async () => {
    try {
      const [docTypesRes, locationsRes, coursesRes, periodsRes, tagsRes] = await Promise.all([
        apiService.get<DocumentTypeDto[]>('/documenttypes', { params: { pageNumber: 1, pageSize: 100 } }),
        apiService.get<LocationDto[]>('/locations', { params: { pageNumber: 1, pageSize: 100 } }),
        apiService.get<CourseDto[]>('/courses', { params: { pageNumber: 1, pageSize: 100 } }),
        apiService.get<AcademicPeriodDto[]>('/academicperiods', { params: { pageNumber: 1, pageSize: 100 } }),
        apiService.get<TagDto[]>('/tags', { params: { pageNumber: 1, pageSize: 100 } }),
      ]);

      setDocumentTypes(docTypesRes.data);
      setLocations(locationsRes.data);
      setCourses(coursesRes.data);
      setAcademicPeriods(periodsRes.data);
      setTags(tagsRes.data);
    } catch (err) {
      console.error('Error loading lookup data:', err);
    }
  };

  const handleCreateClick = () => {
    setFormData({
      title: '',
      description: '',
      documentTypeId: 0,
      locationId: 0,
      courseId: undefined,
      academicPeriodId: undefined,
      tagIds: [],
    });
    setCreateDialogOpen(true);
  };

  const handleCreateSubmit = async () => {
    if (!formData.title || !formData.documentTypeId || !formData.locationId) {
      alert('Please fill in all required fields');
      return;
    }

    setCreateLoading(true);
    try {
      await apiService.post('/documents', formData);
      alert('Document created successfully!');
      setCreateDialogOpen(false);
      fetchDocuments();
    } catch (err: any) {
      alert(err.response?.data?.message || 'An error occurred while creating the document');
    } finally {
      setCreateLoading(false);
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Available':
        return 'bg-green-100 text-green-800 border-green-200';
      case 'CheckedOut':
        return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'Missing':
        return 'bg-red-100 text-red-800 border-red-200';
      default:
        return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'Available':
        return 'Available';
      case 'CheckedOut':
        return 'Checked Out';
      case 'Missing':
        return 'Missing';
      default:
        return status;
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Document Management</h1>
          <p className="mt-2 text-muted-foreground">
            View all documents in the system and add new documents
          </p>
        </div>
        <Button onClick={handleCreateClick}>
          <Plus className="mr-2 h-4 w-4" />
          Add New Document
        </Button>
      </div>

      {error && (
        <div className="rounded-lg bg-destructive/10 p-4 text-sm text-destructive">
          {error}
        </div>
      )}

      {/* Filters */}
      <Card>
        <CardHeader>
          <CardTitle>Filters & Search</CardTitle>
        </CardHeader>
        <CardContent>
          {/* Title Search */}
          <div className="mb-4">
            <Label htmlFor="searchTitle">Search by Title</Label>
            <Input
              id="searchTitle"
              placeholder="Enter document title to search..."
              value={searchTitleInput}
              onChange={(e) => setSearchTitleInput(e.target.value)}
              className="mt-1"
            />
          </div>
          
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
            <div className="space-y-2">
              <Label htmlFor="filterDocumentType">Document Type</Label>
              <Select
                value={filterDocumentTypeId?.toString() || undefined}
                onValueChange={(value) => {
                  setFilterDocumentTypeId(value ? parseInt(value) : undefined);
                  handleFilterChange();
                }}
              >
                <SelectTrigger id="filterDocumentType">
                  <SelectValue placeholder="All document types" />
                </SelectTrigger>
                <SelectContent>
                  {documentTypes.map((dt) => (
                    <SelectItem key={dt.id} value={dt.id.toString()}>
                      {dt.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="filterLocation">Location</Label>
              <Select
                value={filterLocationId?.toString() || undefined}
                onValueChange={(value) => {
                  setFilterLocationId(value ? parseInt(value) : undefined);
                  handleFilterChange();
                }}
              >
                <SelectTrigger id="filterLocation">
                  <SelectValue placeholder="All locations" />
                </SelectTrigger>
                <SelectContent>
                  {locations.map((loc) => (
                    <SelectItem key={loc.id} value={loc.id.toString()}>
                      {loc.friendlyName}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="filterCourse">Course</Label>
              <Select
                value={filterCourseId?.toString() || undefined}
                onValueChange={(value) => {
                  setFilterCourseId(value ? parseInt(value) : undefined);
                  handleFilterChange();
                }}
              >
                <SelectTrigger id="filterCourse">
                  <SelectValue placeholder="All courses" />
                </SelectTrigger>
                <SelectContent>
                  {courses.map((course) => (
                    <SelectItem key={course.id} value={course.id.toString()}>
                      {course.courseCode} - {course.courseName}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="filterStatus">Status</Label>
              <Select
                value={filterStatus || undefined}
                onValueChange={(value) => {
                  setFilterStatus(value || undefined);
                  handleFilterChange();
                }}
              >
                <SelectTrigger id="filterStatus">
                  <SelectValue placeholder="All statuses" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Available">Available</SelectItem>
                  <SelectItem value="CheckedOut">Checked Out</SelectItem>
                  <SelectItem value="Missing">Missing</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="filterAcademicPeriod">Academic Period</Label>
              <Select
                value={filterAcademicPeriodId?.toString() || undefined}
                onValueChange={(value) => {
                  setFilterAcademicPeriodId(value ? parseInt(value) : undefined);
                  handleFilterChange();
                }}
              >
                <SelectTrigger id="filterAcademicPeriod">
                  <SelectValue placeholder="All periods" />
                </SelectTrigger>
                <SelectContent>
                  {academicPeriods.map((period) => (
                    <SelectItem key={period.id} value={period.id.toString()}>
                      {period.periodName}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="filterTag">Tag</Label>
              <Select
                value={filterTagId?.toString() || undefined}
                onValueChange={(value) => {
                  setFilterTagId(value ? parseInt(value) : undefined);
                  handleFilterChange();
                }}
              >
                <SelectTrigger id="filterTag">
                  <SelectValue placeholder="All tags" />
                </SelectTrigger>
                <SelectContent>
                  {tags.map((tag) => (
                    <SelectItem key={tag.id} value={tag.id.toString()}>
                      {tag.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2 flex items-end">
              <Button
                variant="outline"
                onClick={handleClearFilters}
                className="w-full"
              >
                <X className="mr-2 h-4 w-4" />
                Clear
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      {loading ? (
        <div className="flex items-center justify-center py-12">
          <Loader2 className="h-8 w-8 animate-spin text-primary" />
        </div>
      ) : (
        <Card>
          <CardHeader>
            <CardTitle>Documents</CardTitle>
          </CardHeader>
          <CardContent>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Title</TableHead>
                  <TableHead>Document Type</TableHead>
                  <TableHead>Location</TableHead>
                  <TableHead>Course</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Tags</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {documents.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={6} className="text-center py-8">
                      <FileText className="mx-auto h-12 w-12 text-muted-foreground mb-4" />
                      <p className="text-muted-foreground">No documents found</p>
                    </TableCell>
                  </TableRow>
                ) : (
                  documents.map((doc) => (
                    <TableRow key={doc.id}>
                      <TableCell className="font-medium">{doc.title}</TableCell>
                      <TableCell>{doc.documentType?.name || '-'}</TableCell>
                      <TableCell>{doc.location?.friendlyName || '-'}</TableCell>
                      <TableCell>{doc.course?.courseName || '-'}</TableCell>
                      <TableCell>
                        <Badge className={cn('border', getStatusColor(doc.status))}>
                          {getStatusText(doc.status)}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div className="flex flex-wrap gap-1">
                          {doc.tags?.map((tag: TagDto) => (
                            <Badge key={tag.id} variant="outline" className="text-xs">
                              <TagIcon className="mr-1 h-3 w-3" />
                              {tag.name}
                            </Badge>
                          ))}
                        </div>
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>

            {paginationMetadata && paginationMetadata.totalPages > 0 && (
              <div className="mt-4 flex items-center justify-center gap-2 flex-wrap">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setPageNumber(1)}
                  disabled={pageNumber === 1}
                >
                  First
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setPageNumber((p) => Math.max(1, p - 1))}
                  disabled={pageNumber === 1}
                >
                  Previous
                </Button>
                
                {/* Page numbers */}
                <div className="flex items-center gap-1">
                  {Array.from({ length: Math.min(5, paginationMetadata.totalPages) }, (_, i) => {
                    let pageNum: number;
                    if (paginationMetadata.totalPages <= 5) {
                      pageNum = i + 1;
                    } else if (pageNumber <= 3) {
                      pageNum = i + 1;
                    } else if (pageNumber >= paginationMetadata.totalPages - 2) {
                      pageNum = paginationMetadata.totalPages - 4 + i;
                    } else {
                      pageNum = pageNumber - 2 + i;
                    }
                    
                    return (
                      <Button
                        key={pageNum}
                        variant={pageNumber === pageNum ? "default" : "outline"}
                        size="sm"
                        onClick={() => setPageNumber(pageNum)}
                        className="min-w-[40px]"
                      >
                        {pageNum}
                      </Button>
                    );
                  })}
                </div>
                
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setPageNumber((p) => Math.min(paginationMetadata.totalPages, p + 1))}
                  disabled={pageNumber === paginationMetadata.totalPages}
                >
                  Next
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setPageNumber(paginationMetadata.totalPages)}
                  disabled={pageNumber === paginationMetadata.totalPages}
                >
                  Last
                </Button>
                
                <span className="text-sm text-muted-foreground ml-2">
                  Page {pageNumber} of {paginationMetadata.totalPages} ({paginationMetadata.totalCount} total)
                </span>
              </div>
            )}
          </CardContent>
        </Card>
      )}

      {/* Create Document Dialog */}
      <Dialog open={createDialogOpen} onOpenChange={setCreateDialogOpen}>
        <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Add New Document</DialogTitle>
            <DialogDescription>
              Fill out the form below to add a new document to the system
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <Label htmlFor="title">Title *</Label>
              <Input
                id="title"
                value={formData.title}
                onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="description">Description</Label>
              <Input
                id="description"
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="documentType">Document Type *</Label>
              <Select
                value={formData.documentTypeId.toString()}
                onValueChange={(value) => setFormData({ ...formData, documentTypeId: parseInt(value) })}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select document type" />
                </SelectTrigger>
                <SelectContent>
                  {documentTypes.map((dt) => (
                    <SelectItem key={dt.id} value={dt.id.toString()}>
                      {dt.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="location">Location *</Label>
              <Select
                value={formData.locationId.toString()}
                onValueChange={(value) => setFormData({ ...formData, locationId: parseInt(value) })}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select location" />
                </SelectTrigger>
                <SelectContent>
                  {locations.map((loc) => (
                    <SelectItem key={loc.id} value={loc.id.toString()}>
                      {loc.friendlyName}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="course">Course</Label>
              <Select
                value={formData.courseId ? formData.courseId.toString() : undefined}
                onValueChange={(value) => {
                  if (value === '__clear__') {
                    setFormData({ ...formData, courseId: undefined });
                  } else {
                    setFormData({ ...formData, courseId: value ? parseInt(value) : undefined });
                  }
                }}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select course (optional)" />
                </SelectTrigger>
                <SelectContent>
                  {formData.courseId && (
                    <SelectItem value="__clear__">Clear selection</SelectItem>
                  )}
                  {courses.map((course) => (
                    <SelectItem key={course.id} value={course.id.toString()}>
                      {course.courseCode} - {course.courseName}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="academicPeriod">Academic Period</Label>
              <Select
                value={formData.academicPeriodId ? formData.academicPeriodId.toString() : undefined}
                onValueChange={(value) => {
                  if (value === '__clear__') {
                    setFormData({ ...formData, academicPeriodId: undefined });
                  } else {
                    setFormData({ ...formData, academicPeriodId: value ? parseInt(value) : undefined });
                  }
                }}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select academic period (optional)" />
                </SelectTrigger>
                <SelectContent>
                  {formData.academicPeriodId && (
                    <SelectItem value="__clear__">Clear selection</SelectItem>
                  )}
                  {academicPeriods.map((period) => (
                    <SelectItem key={period.id} value={period.id.toString()}>
                      {period.periodName}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="tags">Tags</Label>
              <Select
                value={formData.tagIds.map(String).join(',')}
                onValueChange={(value) => {
                  if (value) {
                    setFormData({ ...formData, tagIds: value.split(',').map(Number) });
                  } else {
                    setFormData({ ...formData, tagIds: [] });
                  }
                }}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select tag" />
                </SelectTrigger>
                <SelectContent>
                  {tags.map((tag) => (
                    <SelectItem key={tag.id} value={tag.id.toString()}>
                      {tag.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <p className="text-xs text-muted-foreground">
                Note: Currently only single tag selection is available. Multiple selection will be added soon.
              </p>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setCreateDialogOpen(false)}>
              Cancel
            </Button>
            <Button
              onClick={handleCreateSubmit}
              disabled={createLoading || !formData.title || !formData.documentTypeId || !formData.locationId}
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

export default AdminDocuments;
