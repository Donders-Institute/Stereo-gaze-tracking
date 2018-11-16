% StereoEyetracker
% Example of the offline gaze reconstruction algorithms for the stereo
% eyetracker, as described in Barsingerhorn, Boonstra & Goossens (2018),
% Behav Res Meth

% Annemiek Barsingerhorn
% Jeroen Goossens (speed optim)
  

%% Load calibration data
ticstart=tic;

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

% Right camera
disp('Loading data from camera 1');
Datainfo1 = [];
fid=fopen(gazelog2);
tline='0';
ind=1;
while tline~=-1
    tline = fgets(fid);
    if tline~=-1
        % convert the text to numbers
        tline(tline==',') = '.';
        Datainfo1{ind}=sscanf(tline,'%f');
        ind=ind+1;
    end
end
fclose(fid);
% convert to array of doubles
Datainfo1 = cell2mat(Datainfo1);
% ticks to milliseconds, each tick lasts 100 nanoseconds
tstamps1  = Datainfo1(1,:)*0.0001; 

% Left camera
disp('Loading data from camera 2');
Datainfo2 = [];
fid=fopen(gazelog1);
tline='0';
ind=1;
while tline~=-1
    tline = fgets(fid);
    if tline~=-1
        % convert the text to numbers
        tline(tline==',') = '.';
        Datainfo2{ind}=sscanf(tline,'%f');
        ind=ind+1;
    end
end
fclose(fid);
% convert to array of doubles
Datainfo2 = cell2mat(Datainfo2);
% ticks to milliseconds
tstamps2  = Datainfo2(1,:)*0.0001; 

%% Select a restricted data set

% Takes a long time, therefore not all samples. 
% First part eyes were not visible and therefore no valid data
tstampstot = unique([tstamps1 tstamps2]);

s1 = tstamps1 >= tstampstot(50000) & tstamps1 <= tstampstot(100000);
s2 = tstamps2 >= tstampstot(50000) & tstamps2 <= tstampstot(100000);
Datainfo1 = Datainfo1(:,s1);
Datainfo2 = Datainfo2(:,s2);
tstamps1  = tstamps1(s1);
tstamps2  = tstamps2(s2);
nSamples1 = length(tstamps1);
nSamples2 = length(tstamps2);

%%  Eliminate duplicate time stamps
[tstamps1, IC]= unique(tstamps1,'first');
Datainfo1 = Datainfo1(:,IC);

[tstamps2, IC]= unique(tstamps2,'first');
Datainfo2 = Datainfo2(:,IC);

%% Extract relevant data and undistort and unscramble coordinates

%JG: undistortPoints is not very fast. Therefore it is better/faster to do it here before the interpolation
Leftglint1cam1r  = Datainfo1(6:7,:);
Leftglint2cam1r  = Datainfo1(8:9,:); 
Rightglint1cam1r = Datainfo1(10:11,:);
Rightglint2cam1r = Datainfo1(12:13,:); 
Leftpupilcam1r   = Datainfo1(14:15,:);
Rightpupilcam1r  = Datainfo1(16:17,:);
Leftpupsizecam1  = Datainfo1(18,:);
Rightpupsizecam1 = Datainfo1(19,:);

disp('Undistort and unscramble data from camera 1'); tic
parfor i=1:nSamples1
    
    % make sure left eye is always on left side
    pl = Leftpupilcam1r(:,i);
    pr = Rightpupilcam1r(:,i);
    if pl(1) < pr(1)
        pnts = [Leftglint1cam1r(:,i) Leftglint2cam1r(:,i) Rightglint1cam1r(:,i) Rightglint2cam1r(:,i) Leftpupilcam1r(:,i) Rightpupilcam1r(:,i)]';
    else
        pnts = [Rightglint1cam1r(:,i) Rightglint2cam1r(:,i) Leftglint1cam1r(:,i) Leftglint2cam1r(:,i) Rightpupilcam1r(:,i) Leftpupilcam1r(:,i)]';
    end
    % undistort
    pnts = undistortPoints( pnts ,params.CameraParameters2);
    
    % make sure glint1 is to the left of glint2
    if pnts(1,1) < pnts(2,1)
        Leftglint1cam1r(:,i)  = pnts(1,:);
        Leftglint2cam1r(:,i)  = pnts(2,:);
    else
        Leftglint1cam1r(:,i)  = pnts(2,:);
        Leftglint2cam1r(:,i)  = pnts(1,:);
    end
    if pnts(3,1) < pnts(4,1)
        Rightglint1cam1r(:,i) = pnts(3,:);
        Rightglint2cam1r(:,i) = pnts(4,:);
    else
        Rightglint1cam1r(:,i) = pnts(4,:);
        Rightglint2cam1r(:,i) = pnts(3,:);
    end
    Leftpupilcam1r(:,i)     = pnts(5,:);
    Rightpupilcam1r(:,i)    = pnts(6,:);
end
toc

Leftglint1cam2r  = Datainfo2(6:7,:); 
Leftglint2cam2r  = Datainfo2(8:9,:); 
Rightglint1cam2r = Datainfo2(10:11,:);
Rightglint2cam2r = Datainfo2(12:13,:); 
Leftpupilcam2r   = Datainfo2(14:15,:);
Rightpupilcam2r  = Datainfo2(16:17,:);
Leftpupsizecam2  = Datainfo2(18,:);
Rightpupsizecam2 = Datainfo2(19,:);

disp('Undistort and unscramble data from camera 2'); tic
parfor i=1:nSamples2
    
    % make sure left eye is always on left side
    pl = Leftpupilcam2r(:,i);
    pr = Rightpupilcam2r(:,i);
    if pl(1) < pr(1)
        pnts = [Leftglint1cam2r(:,i) Leftglint2cam2r(:,i) Rightglint1cam2r(:,i) Rightglint2cam2r(:,i) Leftpupilcam2r(:,i) Rightpupilcam2r(:,i)]';
    else
        pnts = [Rightglint1cam2r(:,i) Rightglint2cam2r(:,i) Leftglint1cam2r(:,i) Leftglint2cam2r(:,i) Rightpupilcam2r(:,i) Leftpupilcam2r(:,i)]';
    end
    % undistort
    pnts = undistortPoints( pnts ,params.CameraParameters1);
    
    % make sure glint1 is to the left of glint2
    if pnts(1,1) < pnts(2,1)
        Leftglint1cam2r(:,i)  = pnts(1,:);
        Leftglint2cam2r(:,i)  = pnts(2,:);
    else
        Leftglint1cam2r(:,i)  = pnts(2,:);
        Leftglint2cam2r(:,i)  = pnts(1,:);
    end
    if pnts(3,1) < pnts(4,1)
        Rightglint1cam2r(:,i) = pnts(3,:);
        Rightglint2cam2r(:,i) = pnts(4,:);
    else
        Rightglint1cam2r(:,i) = pnts(4,:);
        Rightglint2cam2r(:,i) = pnts(3,:);
    end
    Leftpupilcam2r(:,i)     = pnts(5,:);
    Rightpupilcam2r(:,i)    = pnts(6,:);
end
toc

% LocL1 and LocL2 are the positions of the IR sources. 
% Make sure there were no left/right switches during calibration
if LocL1(1) < LocL2(1)
    l1=LocL1';
    l2=LocL2';
else
    l2=LocL1';
    l1=LocL2';
end


%% Interpolate signals

tstampstot = unique([tstamps1 tstamps2]);
nSamples   = length(tstampstot);

Leftpupilcam1   = zeros(2,nSamples);
Rightpupilcam1  = zeros(2,nSamples);
Leftglint1cam1  = zeros(2,nSamples);
Leftglint2cam1  = zeros(2,nSamples);
Rightglint1cam1 = zeros(2,nSamples);
Rightglint2cam1 = zeros(2,nSamples);
Leftpupilcam2   = zeros(2,nSamples);
Rightpupilcam2  = zeros(2,nSamples);
Leftglint1cam2  = zeros(2,nSamples);
Leftglint2cam2  = zeros(2,nSamples);
Rightglint1cam2 = zeros(2,nSamples);
Rightglint2cam2 = zeros(2,nSamples);

Leftpupilcam1(1,:) = interp1(tstamps1,Leftpupilcam1r(1,:),tstampstot,'linear','extrap');
Leftpupilcam2(1,:) = interp1(tstamps2,Leftpupilcam2r(1,:),tstampstot,'linear','extrap');

Rightpupilcam1(1,:) = interp1(tstamps1,Rightpupilcam1r(1,:),tstampstot,'linear','extrap');
Rightpupilcam2(1,:) = interp1(tstamps2,Rightpupilcam2r(1,:),tstampstot,'linear','extrap');

Leftglint1cam1(1,:)  = interp1( tstamps1(~isnan(Leftglint1cam1r(1,:))), Leftglint1cam1r(1,(~isnan(Leftglint1cam1r(1,:)))), tstampstot,'linear','extrap');
Leftglint2cam1(1,:)  = interp1( tstamps1(~isnan(Leftglint2cam1r(1,:))), Leftglint2cam1r(1,(~isnan(Leftglint2cam1r(1,:)))), tstampstot,'linear','extrap');
Rightglint1cam1(1,:) = interp1( tstamps1(~isnan(Rightglint1cam1r(1,:))), Rightglint1cam1r(1,(~isnan(Rightglint1cam1r(1,:)))), tstampstot,'linear','extrap');
Rightglint2cam1(1,:) = interp1( tstamps1(~isnan(Rightglint2cam1r(1,:))), Rightglint2cam1r(1,(~isnan(Rightglint2cam1r(1,:)))), tstampstot,'linear','extrap');

Leftglint1cam2(1,:)  = interp1( tstamps2(~isnan(Leftglint1cam2r(1,:))), Leftglint1cam2r(1,(~isnan(Leftglint1cam2r(1,:)))), tstampstot,'linear','extrap');
Leftglint2cam2(1,:)  = interp1( tstamps2(~isnan(Leftglint2cam2r(1,:))), Leftglint2cam2r(1,(~isnan(Leftglint2cam2r(1,:)))), tstampstot,'linear','extrap');
Rightglint1cam2(1,:) = interp1( tstamps2(~isnan(Rightglint1cam2r(1,:))), Rightglint1cam2r(1,(~isnan(Rightglint1cam2r(1,:)))), tstampstot,'linear','extrap');
Rightglint2cam2(1,:) = interp1( tstamps2(~isnan(Rightglint2cam2r(1,:))), Rightglint2cam2r(1,(~isnan(Rightglint2cam2r(1,:)))), tstampstot,'linear','extrap');

Leftpupilcam1(2,:) = interp1(tstamps1,Leftpupilcam1r(2,:),tstampstot,'linear','extrap');
Leftpupilcam2(2,:) = interp1(tstamps2,Leftpupilcam2r(2,:),tstampstot,'linear','extrap');

Rightpupilcam1(2,:) = interp1(tstamps1,Rightpupilcam1r(2,:),tstampstot,'linear','extrap');
Rightpupilcam2(2,:) = interp1(tstamps2,Rightpupilcam2r(2,:),tstampstot,'linear','extrap');

Leftglint1cam1(2,:)  = interp1( tstamps1(~isnan(Leftglint1cam1r(1,:))), Leftglint1cam1r(2,(~isnan(Leftglint1cam1r(1,:)))), tstampstot,'linear','extrap');
Leftglint2cam1(2,:)  = interp1( tstamps1(~isnan(Leftglint2cam1r(1,:))), Leftglint2cam1r(2,(~isnan(Leftglint2cam1r(1,:)))), tstampstot,'linear','extrap');
Rightglint1cam1(2,:) = interp1( tstamps1(~isnan(Rightglint1cam1r(1,:))), Rightglint1cam1r(2,(~isnan(Rightglint1cam1r(1,:)))), tstampstot,'linear','extrap');
Rightglint2cam1(2,:) = interp1( tstamps1(~isnan(Rightglint2cam1r(1,:))), Rightglint2cam1r(2,(~isnan(Rightglint2cam1r(1,:)))), tstampstot,'linear','extrap');

Leftglint1cam2(2,:)  = interp1( tstamps2(~isnan(Leftglint1cam2r(1,:))), Leftglint1cam2r(2,(~isnan(Leftglint1cam2r(1,:)))), tstampstot,'linear','extrap');
Leftglint2cam2(2,:)  = interp1( tstamps2(~isnan(Leftglint2cam2r(1,:))), Leftglint2cam2r(2,(~isnan(Leftglint2cam2r(1,:)))), tstampstot,'linear','extrap');
Rightglint1cam2(2,:) = interp1( tstamps2(~isnan(Rightglint1cam2r(1,:))), Rightglint1cam2r(2,(~isnan(Rightglint1cam2r(1,:)))), tstampstot,'linear','extrap');
Rightglint2cam2(2,:) = interp1( tstamps2(~isnan(Rightglint2cam2r(1,:))), Rightglint2cam2r(2,(~isnan(Rightglint2cam2r(1,:)))), tstampstot,'linear','extrap');


%% Estimate center virtual pupil and center of corneal curvature

disp('Estimate 3D coordinates of pupil and center of corneal curvature'); tic

normleft = zeros(nSamples,3); 
cleft    = zeros(3,nSamples); 
pleft    = zeros(3,nSamples); 
normright = zeros(nSamples,3); 
cright    = zeros(3,nSamples); 
pright    = zeros(3,nSamples); 

parfor i=1:nSamples
    [normleft(i,:) ,cleft(:,i) ,pleft(:,i) ]=estimatepandc(params,T1,l1,l2,Leftpupilcam1(:,i) ,Leftpupilcam2(:,i) ,Leftglint1cam1(:,i) ,Leftglint1cam2(:,i) ,Leftglint2cam1(:,i) ,Leftglint2cam2(:,i) );
    [normright(i,:),cright(:,i),pright(:,i)]=estimatepandc(params,T1,l1,l2,Rightpupilcam1(:,i),Rightpupilcam2(:,i),Rightglint1cam1(:,i),Rightglint1cam2(:,i),Rightglint2cam1(:,i),Rightglint2cam2(:,i));
end
toc

%% Calculate gaze
% alpha, beta and K were estimated in a one-point calibration procedure
Kleft=4.56;
Kright=4.36;

alphaleft=4.65;
alpharight=-2.54;

betaleft=1.61;
betaright=3.58;

disp('Calculate gaze'); tic

[Pcorrectleft, Ccorrectleft, normcorrectleft, ax1lef,  ax2left, cgazeleft, gazeleft, lengthnleft ]=calculategaze(cleft, pleft, Rpos,Centerscreen,Kleft,alphaleft,betaleft);
[Pcorrectright,Ccorrectright,normcorrectright,ax1right,ax2right,cgazeright,gazeright,lengthnright]=calculategaze(cright,pright,Rpos,Centerscreen,Kright,alpharight,betaright);

% Correction with K/Kactual, as described in the paper
gazeleftcorrected =(Kleft./medfilt1(lengthnleft,20))'.*gazeleft;
gazerightcorrected=(Kright./medfilt1(lengthnright,20))'.*gazeright;
toc

disp('All done');
toc(ticstart)

% Plots
plot(tstampstot,gazeleft)
hold on
plot(tstampstot,gazeleftcorrected,'c')

plot(tstampstot,gazeright,'m')
plot(tstampstot,gazerightcorrected,'k')
ylim([-512 +512])
hold off




   