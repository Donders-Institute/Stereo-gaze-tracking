% StereoEyetracker
% Example of the offline gaze reconstruction algorithms for the stereo
% eyetracker, as described in Barsingerhorn, Boonstra & Goossens (2018),
% Behav Res Meth

% Annemiek Barsingerhorn
  

%% Load calibration data
load('StereocalibMatlab.mat');
load('setup.mat')

% Position cam 2 with respect to cam 1
T1=params.RotationOfCamera2*-params.TranslationOfCamera2'/10;

% Rotation matrix screen
Rpos=m;

%% Read data StereoEyetracker

% Names data files
gazelog1='gazelog1cam2016121911577740.txt';
gazelog2='gazelog2cam2016121911572441.txt';

fid=fopen(gazelog2);
tline='0';
ind=1;
while tline~=-1
    tline = fgets(fid);
    tline(tline==',') = '.';
    data{ind,1}=tline;
    
    ind=ind+1;
end

ind=1;
parfor i=1:length(data)-1
    Datainfo{i}=sscanf(data{i},'%f');
end

% Right camera

% Read data gazetracker
fid=fopen(gazelog1);

tline='0';
ind=1;
while tline~=-1
    tline = fgets(fid);
    tline(tline==',') = '.';
    data2{ind,1}=tline;
    
    ind=ind+1;
end

ind=1;
parfor i=1:length(data2)-1
    Datainfo2{i}=sscanf(data2{i},'%f');
end

parfor i=1:length(Datainfo)
    vr=Datainfo{i};
    var=vr(1);
    sdt=System.DateTime(var);
    Year=double(sdt.Year);
    Month=double(sdt.Month);
    Day=double(sdt.Day);
    Hour=double(sdt.Hour);
    Minute=double(sdt.Minute);
    Second=double(sdt.Second);
    Millisecond=double(sdt.Millisecond)/1000;
    tstamps(i)=datenum( Year,Month,Day,Hour,Minute,Second+Millisecond)*86400000;
end

parfor i=1:length(Datainfo2)
    vr=Datainfo2{i};
    var=vr(1);
    sdt=System.DateTime(var);
    Year=double(sdt.Year);
    Month=double(sdt.Month);
    Day=double(sdt.Day);
    Hour=double(sdt.Hour);
    Minute=double(sdt.Minute);
    Second=double(sdt.Second);
    Millisecond=double(sdt.Millisecond)/1000;
    tstamps2(i)=datenum( Year,Month,Day,Hour,Minute,Second+Millisecond)*86400000;
end
    
%% Extract relevant data and check data
 % make sure left eye is always on left side, left glint in cam 1 correspond
 % with left glint in cam 2 (and not the right glint) etc.
Leftglint1cam1unf = [cellfun(@(x) x(6), Datainfo); cellfun(@(x) x(7), Datainfo)];
Leftglint2cam1unf = [cellfun(@(x) x(8), Datainfo); cellfun(@(x) x(9), Datainfo)];
Rightglint1cam1unf = [cellfun(@(x) x(10), Datainfo); cellfun(@(x) x(11), Datainfo)];
Rightglint2cam1unf = [cellfun(@(x) x(12), Datainfo); cellfun(@(x) x(13), Datainfo)];
Leftpupilcam1r = [cellfun(@(x) x(14), Datainfo); cellfun(@(x) x(15), Datainfo)];
Rightpupilcam1r = [cellfun(@(x) x(16), Datainfo); cellfun(@(x) x(17), Datainfo)];

Leftpupsizecam1=[cellfun(@(x) x(18), Datainfo)];
Rightpupsizecam1=[cellfun(@(x) x(19), Datainfo)];

Leftpupsizecam2=[cellfun(@(x) x(18), Datainfo2)];
Rightpupsizecam2=[cellfun(@(x) x(19), Datainfo2)];

Leftglint1cam2unf = [cellfun(@(x) x(6), Datainfo2); cellfun(@(x) x(7), Datainfo2)];
Leftglint2cam2unf = [cellfun(@(x) x(8), Datainfo2); cellfun(@(x) x(9), Datainfo2)];
Rightglint1cam2unf = [cellfun(@(x) x(10), Datainfo2); cellfun(@(x) x(11), Datainfo2)];
Rightglint2cam2unf = [cellfun(@(x) x(12), Datainfo2); cellfun(@(x) x(13), Datainfo2)];
Leftpupilcam2r = [cellfun(@(x) x(14), Datainfo2); cellfun(@(x) x(15), Datainfo2)];
Rightpupilcam2r = [cellfun(@(x) x(16), Datainfo2); cellfun(@(x) x(17), Datainfo2)];

Leftpupilcam1unf=Leftpupilcam1r;
Leftpupilcam2unf=Leftpupilcam2r;
Rightpupilcam1unf=Rightpupilcam1r;
Rightpupilcam2unf=Rightpupilcam2r;

Leftpupilcam1r(:,Leftpupilcam1unf(1,:)>Rightpupilcam1unf(1,:))=Rightpupilcam1unf(:,Leftpupilcam1unf(1,:)>Rightpupilcam1unf(1,:));
Leftpupilcam2r(:,Leftpupilcam2unf(1,:)>Rightpupilcam2unf(1,:))=Rightpupilcam2unf(:,Leftpupilcam2unf(1,:)>Rightpupilcam2unf(1,:));

Rightpupilcam1r(:,Leftpupilcam1unf(1,:)>Rightpupilcam1unf(1,:))=Leftpupilcam1unf(:,Leftpupilcam1unf(1,:)>Rightpupilcam1unf(1,:));
Rightpupilcam2r(:,Leftpupilcam2unf(1,:)>Rightpupilcam2unf(1,:))=Leftpupilcam2unf(:,Leftpupilcam2unf(1,:)>Rightpupilcam2unf(1,:));


Leftglint1cam1r=Leftglint1cam1unf;
Leftglint1cam1r(:,Leftglint1cam1unf(1,:)>Leftglint2cam1unf(1,:))=Leftglint2cam1unf(:,Leftglint1cam1unf(1,:)>Leftglint2cam1unf(1,:));
Leftglint2cam1r=Leftglint2cam1unf;
Leftglint2cam1r(:,Leftglint1cam1unf(1,:)>Leftglint2cam1unf(1,:))=Leftglint1cam1unf(:,Leftglint1cam1unf(1,:)>Leftglint2cam1unf(1,:));

Rightglint1cam1r=Rightglint1cam1unf;
Rightglint1cam1r(:,Rightglint1cam1unf(1,:)>Rightglint2cam1unf(1,:))=Rightglint2cam1unf(:,Rightglint1cam1unf(1,:)>Rightglint2cam1unf(1,:));
Rightglint2cam1r=Rightglint2cam1unf;
Rightglint2cam1r(:,Rightglint1cam1unf(1,:)>Rightglint2cam1unf(1,:))=Rightglint1cam1unf(:,Rightglint1cam1unf(1,:)>Rightglint2cam1unf(1,:));

Leftglint1cam2r=Leftglint1cam2unf;
Leftglint1cam2r(:,Leftglint1cam2unf(1,:)>Leftglint2cam2unf(1,:))=Leftglint2cam2unf(:,Leftglint1cam2unf(1,:)>Leftglint2cam2unf(1,:));
Leftglint2cam2r=Leftglint2cam2unf;
Leftglint2cam2r(:,Leftglint1cam2unf(1,:)>Leftglint2cam2unf(1,:))=Leftglint1cam2unf(:,Leftglint1cam2unf(1,:)>Leftglint2cam2unf(1,:));

Rightglint1cam2r=Rightglint1cam2unf;
Rightglint1cam2r(:,Rightglint1cam2unf(1,:)>Rightglint2cam2unf(1,:))=Rightglint2cam2unf(:,Rightglint1cam2unf(1,:)>Rightglint2cam2unf(1,:));
Rightglint2cam2r=Rightglint2cam2unf;
Rightglint2cam2r(:,Rightglint1cam2unf(1,:)>Rightglint2cam2unf(1,:))=Rightglint1cam2unf(:,Rightglint1cam2unf(1,:)>Rightglint2cam2unf(1,:));


Leftglint1cam1ru=Leftglint1cam1r;
Rightglint1cam1ru=Rightglint1cam1r;
Leftglint2cam1ru=Leftglint2cam1r;
Rightglint2cam1ru=Rightglint2cam1r;

Leftglint1cam2ru=Leftglint1cam2r;
Rightglint1cam2ru=Rightglint1cam2r;
Leftglint2cam2ru=Leftglint2cam2r;
Rightglint2cam2ru=Rightglint2cam2r;

Leftglint1cam1r(:,Leftpupilcam1unf(1,:)>Rightpupilcam1unf(1,:))=Rightglint1cam1ru(:,Leftpupilcam1unf(1,:)>Rightpupilcam1unf(1,:));
Rightglint1cam1r(:,Leftpupilcam1unf(1,:)>Rightpupilcam1unf(1,:))=Leftglint1cam1ru(:,Leftpupilcam1unf(1,:)>Rightpupilcam1unf(1,:));

Leftglint2cam1r(:,Leftpupilcam1unf(1,:)>Rightpupilcam1unf(1,:))=Rightglint2cam1ru(:,Leftpupilcam1unf(1,:)>Rightpupilcam1unf(1,:));
Rightglint2cam1r(:,Leftpupilcam1unf(1,:)>Rightpupilcam1unf(1,:))=Leftglint2cam1ru(:,Leftpupilcam1unf(1,:)>Rightpupilcam1unf(1,:));

Leftglint1cam2r(:,Leftpupilcam2unf(1,:)>Rightpupilcam2unf(1,:))=Rightglint1cam2ru(:,Leftpupilcam2unf(1,:)>Rightpupilcam2unf(1,:));
Rightglint1cam2r(:,Leftpupilcam2unf(1,:)>Rightpupilcam2unf(1,:))=Leftglint1cam2ru(:,Leftpupilcam2unf(1,:)>Rightpupilcam2unf(1,:));

Leftglint2cam2r(:,Leftpupilcam2unf(1,:)>Rightpupilcam2unf(1,:))=Rightglint2cam2ru(:,Leftpupilcam2unf(1,:)>Rightpupilcam2unf(1,:));
Rightglint2cam2r(:,Leftpupilcam2unf(1,:)>Rightpupilcam2unf(1,:))=Leftglint2cam2ru(:,Leftpupilcam2unf(1,:)>Rightpupilcam2unf(1,:));

t0=tstamps(ismember(tstamps,tstamps2));

tstampstot=unique([tstamps tstamps2]);

%% Interpolate signals
indd=find(diff(tstamps)==0);
indd2=find(diff(tstamps2)==0);

tstamps(indd)=tstamps(indd)+0.1;
tstamps2(indd2)=tstamps2(indd2)+0.1;

Leftpupilcam1(1,:) = (interp1(tstamps,((Leftpupilcam1r(1,:))),tstampstot,'linear'));
Leftpupilcam2(1,:) = (interp1(tstamps2,((Leftpupilcam2r(1,:))),tstampstot,'linear'));

Rightpupilcam1(1,:) = (interp1(tstamps,((Rightpupilcam1r(1,:))),tstampstot,'linear'));
Rightpupilcam2(1,:) = (interp1(tstamps2,((Rightpupilcam2r(1,:))),tstampstot,'linear'));

Leftglint1cam1(1,:) = (interp1(tstamps(~isnan(Leftglint1cam1r(1,:))),((Leftglint1cam1r(1,(~isnan(Leftglint1cam1r(1,:)))))),tstampstot,'linear'));
Leftglint2cam1(1,:) = (interp1(tstamps(~isnan(Leftglint2cam1r(1,:))),((Leftglint2cam1r(1,(~isnan(Leftglint2cam1r(1,:)))))),tstampstot,'linear'));
Rightglint1cam1(1,:) = (interp1(tstamps(~isnan(Rightglint1cam1r(1,:))),((Rightglint1cam1r(1,(~isnan(Rightglint1cam1r(1,:)))))),tstampstot,'linear'));
Rightglint2cam1(1,:) = (interp1(tstamps(~isnan(Rightglint2cam1r(1,:))),((Rightglint2cam1r(1,(~isnan(Rightglint2cam1r(1,:)))))),tstampstot,'linear'));

Leftglint1cam2(1,:) = (interp1(tstamps2(~isnan(Leftglint1cam2r(1,:))),((Leftglint1cam2r(1,(~isnan(Leftglint1cam2r(1,:)))))),tstampstot,'linear'));
Leftglint2cam2(1,:) = (interp1(tstamps2(~isnan(Leftglint2cam2r(1,:))),((Leftglint2cam2r(1,(~isnan(Leftglint2cam2r(1,:)))))),tstampstot,'linear'));
Rightglint1cam2(1,:) =(interp1(tstamps2(~isnan(Rightglint1cam2r(1,:))),((Rightglint1cam2r(1,(~isnan(Rightglint1cam2r(1,:)))))),tstampstot,'linear'));
Rightglint2cam2(1,:) = (interp1(tstamps2(~isnan(Rightglint2cam2r(1,:))),((Rightglint2cam2r(1,(~isnan(Rightglint2cam2r(1,:)))))),tstampstot,'linear'));

Leftpupilcam1(2,:) = (interp1(tstamps,((Leftpupilcam1r(2,:))),tstampstot,'linear'));
Leftpupilcam2(2,:) = (interp1(tstamps2,((Leftpupilcam2r(2,:))),tstampstot,'linear'));

Rightpupilcam1(2,:) = (interp1(tstamps,((Rightpupilcam1r(2,:))),tstampstot,'linear'));
Rightpupilcam2(2,:) = (interp1(tstamps2,((Rightpupilcam2r(2,:))),tstampstot,'linear'));

Leftglint1cam1(2,:) = (interp1(tstamps(~isnan(Leftglint1cam1r(1,:))),((Leftglint1cam1r(2,(~isnan(Leftglint1cam1r(1,:)))))),tstampstot,'linear'));
Leftglint2cam1(2,:) = (interp1(tstamps(~isnan(Leftglint2cam1r(1,:))),((Leftglint2cam1r(2,(~isnan(Leftglint2cam1r(1,:)))))),tstampstot,'linear'));
Rightglint1cam1(2,:) = (interp1(tstamps(~isnan(Rightglint1cam1r(1,:))),((Rightglint1cam1r(2,(~isnan(Rightglint1cam1r(1,:)))))),tstampstot,'linear'));
Rightglint2cam1(2,:) = (interp1(tstamps(~isnan(Rightglint2cam1r(1,:))),((Rightglint2cam1r(2,(~isnan(Rightglint2cam1r(1,:)))))),tstampstot,'linear'));

Leftglint1cam2(2,:) = (interp1(tstamps2(~isnan(Leftglint1cam2r(1,:))),((Leftglint1cam2r(2,(~isnan(Leftglint1cam2r(1,:)))))),tstampstot,'linear'));
Leftglint2cam2(2,:) = (interp1(tstamps2(~isnan(Leftglint2cam2r(1,:))),((Leftglint2cam2r(2,(~isnan(Leftglint2cam2r(1,:)))))),tstampstot,'linear'));
Rightglint1cam2(2,:) = (interp1(tstamps2(~isnan(Rightglint1cam2r(1,:))),((Rightglint1cam2r(2,(~isnan(Rightglint1cam2r(1,:)))))),tstampstot,'linear'));
Rightglint2cam2(2,:) = (interp1(tstamps2(~isnan(Rightglint2cam2r(1,:))),((Rightglint2cam2r(2,(~isnan(Rightglint2cam2r(1,:)))))),tstampstot,'linear'));

%% Estimate center virtual pupil and center of corneal curvature

ind=1:length(tstampstot);
ind2=1:length(tstampstot);

ki=1:length(ind);
clear Par* nl* snorm
in=1;

% LocL1 and LocL2 are the positions of the IR sources. Make sure there were
% no left/right switches during calibration
if LocL1(1) < LocL2(1)
    l1=LocL1';
    l2=LocL2';
else
    l2=LocL1';
    l1=LocL2';
end

% Takes a long time, therefore not all samples. First part eyes were not
% visible and therefore no valid data
parfor i=50000:100000
     
    [normleft(i,:),cleft(:,i),pleft(:,i)]=estimatepandc(params,T1,l1,l2,Leftpupilcam1(:,ind(ki(i))),Leftpupilcam2(:,ind2(ki(i))),...
        Leftglint1cam1(:,ind(ki(i))),Leftglint1cam2(:,ind2(ki(i))),Leftglint2cam1(:,ind(ki(i))),Leftglint2cam2(:,ind2(ki(i))));
    
      [snormright(i,:),cright(:,i),pright(:,i)]=estimatepandc(params,T1,l1,l2,Rightpupilcam1(:,ind(ki(i))),Rightpupilcam2(:,ind2(ki(i))),...
        Rightglint1cam1(:,ind(ki(i))),Rightglint1cam2(:,ind2(ki(i))),Rightglint2cam1(:,ind(ki(i))),Rightglint2cam2(:,ind2(ki(i))));
    
end

%% Calculate gaze
% alpha, beta and K were estimated in a one-point calibration procedure
Kleft=4.56;
Kright=4.36;

alphaleft=4.65;
alpharight=-2.54;

betaleft=1.61;
betaright=3.58;



[Pcorrectleft,Ccorrectleft,normcorrectleft,ax1lef,ax2,cgazeleft,gazeleft,lengthnleft ]=calculategaze(cleft,pleft,Rpos,Centerscreen,Kleft,alphaleft,betaleft);
[Pcorrectright,Ccorrectright,normcorrectright,ax1right,ax2right,cgazeright,gazeright,lengthnright ]=calculategaze(cright,pright,Rpos,Centerscreen,Kright,alpharight,betaright);


% Correction with K/Kactual, as described in the paper
gazeleftcorrected=(Kleft./medfilt1(lengthnleft,20))'.*gazeleft;
gazerightcorrected=(Kright./medfilt1(lengthnright,20))'.*gazeright;

% Plots
plot(tstampstot(50000:100000),gazeleft(50000:100000,:))
hold on
plot(tstampstot(50000:100000),gazeleftcorrected(50000:100000,:),'c')
plot(tstampstot(50000:100000),gazeright(50000:100000,:),'m')
plot(tstampstot(50000:100000),gazerightcorrected(50000:100000,:),'k')




   