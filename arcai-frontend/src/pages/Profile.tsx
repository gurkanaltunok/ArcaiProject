import React from 'react';
import { useAuth } from '../context/AuthContext';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../components/ui/card';
import { Avatar, AvatarFallback } from '../components/ui/avatar';
import { Badge } from '../components/ui/badge';
import { User, Mail, Shield, Calendar } from 'lucide-react';

const Profile: React.FC = () => {
  const { user, role } = useAuth();

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Profile</h1>
        <p className="mt-2 text-muted-foreground">
          View your profile information
        </p>
      </div>

      <Card>
        <CardHeader>
          <div className="flex items-center gap-4">
            <Avatar className="h-20 w-20">
              <AvatarFallback className="text-2xl">
                {user?.firstName?.[0] || 'U'}{user?.lastName?.[0] || ''}
              </AvatarFallback>
            </Avatar>
            <div>
              <CardTitle className="text-2xl">
                {user?.firstName || 'User'} {user?.lastName || ''}
              </CardTitle>
              <CardDescription className="mt-1">
                <Badge variant="secondary" className="text-sm">
                  {role || 'User'}
                </Badge>
              </CardDescription>
            </div>
          </div>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex items-center gap-3">
            <Mail className="h-5 w-5 text-muted-foreground" />
            <div>
              <div className="text-sm font-medium text-muted-foreground">Email</div>
              <div className="text-base">{user?.email || '-'}</div>
            </div>
          </div>

          <div className="flex items-center gap-3">
            <Shield className="h-5 w-5 text-muted-foreground" />
            <div>
              <div className="text-sm font-medium text-muted-foreground">Role</div>
              <div className="text-base">
                <Badge variant="outline">{role || '-'}</Badge>
              </div>
            </div>
          </div>

          <div className="flex items-center gap-3">
            <User className="h-5 w-5 text-muted-foreground" />
            <div>
              <div className="text-sm font-medium text-muted-foreground">User ID</div>
              <div className="text-base">{user?.id || '-'}</div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default Profile;
