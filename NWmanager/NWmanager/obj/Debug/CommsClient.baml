   M S B A M L   `   `   `  ����  	NWmanager    NWmanager.CommsClient   5   	   W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5        � Title$  CommsClient��5      $	��660��6   $	��902��6&   .��5      � .+�5   
   �� -   5        $��txtInfo��5   (   $��12,41,262,73q�6    �� TextWrapping$
 Wrap=�67   " �� VerticalScrollBarVisibility$
 Auto=�6K   �� 5   
   -   5        $��txtIpAddress��5   1   $��23��6   $��	37,12,0,0q�6   $	��Top=�6E   $
��Left=�6]   $��68��6x   .��6�   	127.0.0.16�   �� 5   
   -   5        $��lblIpAddress��5   J   $��23��6   $
��Left=�6   $��	12,12,0,0q�67   $	��Top=�6^   $��33��6v   .��6�   IP:6�   �� 5	   
   -   5        $��lblPort��5	   0   $��28��6   $��
130,12,0,0q�6   $	��Top=�6?   $
��Left=�6W   $��41��6r   .��6}   Port:6�   �� 5
   
   -   5        $��txtPort��5
   2   $��23��6   $��
168,12,0,0q�6   $	��Top=�6A   $
��Left=�6Y   $��50��6t   .��6   20556�   �� 5   
   -   5        $��
btnConnect��5   1   $��23��6   $��
232,12,0,0q�6   $	��Top=�6C   $
��Left=�6t   $��75��6�   .��6�   	Connect6�   �� 5   
   -   5        $��btnReqGameInfo��5   K   $��23��6   $
��Left=�6   $��	12,0,0,44q�68   $��Bottom=�6a   $��65��6|   .��6�   
GameInfo6�   �� 5   
   -   5        $��
btnEndGame��5   0   $��23��6   $��	12,0,0,10q�6   $��Bottom=�6B   $
��Left=�6v   $��65��6�   .��6�   	Terminate6�   �� 5   
   -	   5        $��lstUnits��5   '   $��0,67,12,318q�6   $��Right=�6d   $	��244��6�   �� 5   
   -
   5        $��lblUnits��5   0   $��31��6   $��
0,40,121,0q�6   $	��Top=�6@   $��Right=�6X   $��135.645��6t   .��6�   Units6�   �� 5   
   -   5        $��lstDetections��5   %   $��	0,0,10,73q�6   $��Right=�6�   $	��246��6�   $	��217��6�   $��Bottom=�6�   �� 5   
   -   5        $��lblDetections��5   %   $��0,0,121,283q�6   $��Right=�6:   $��135.343��6V   $��29��6f   $��Bottom=�6r   .��6�   
Detections6�   �� 5   
   -   5        $��
lblPosInfo��5   0   $��28��6   $��
0,0,338,39q�6   $��Bottom=�6B   $��Right=�6]   $��74.781��6y   .��6�   
Lat/Long6�   �� 5   
   -   5        $��txtLat��5   N   $��23��6   $��Right=�6   $��
0,0,304,39q�6:   $��Bottom=�6\   $��38��6w   $��68��6�   6�   �� 5   
   -   5        $��txtLon��5   N   $��23��6   $��Right=�6   $��
0,0,259,39q�6:   $��Bottom=�6\   $��38��6w   $��-9��6�   6�   �� 5   
   -   5        $��txtUnitInfo��5   M   $��47��6   $��Right=�6   $��	0,0,10,12q�6:   $��Bottom=�6`   $	��243��6{   �� 5   
   -   5        $��btnMove��5   M   $��23��6   $��Right=�6   $��
0,0,257,10q�69   $��Bottom=�6\   $��85��6w   .��6�   Move6�   �� 5   
   -   5        $��btnResetLists��5   L   $��23��6   $��Right=�6   $��	0,38,12,0q�69   $	��Top=�6a   $��24��6y   .��6�   X6�   �� 5   
   -   5        $��	btnEngage��5   3   $��23��6   $��380,0,419,10q�6   $��Bottom=�6D   .��6w   Engage6   �� 5   
   -   5        $��lblInfoPanel��5   K   $��23��6   $��Right=�6   $��	0,11,10,0q�68   $	��Top=�6_   $	��257��6w   .��6�   Connect to server...6�   �� 5   
   -   5        $��btnLaunchAircraft��5   L   $��23��6   $
��Left=�6   $��
214,0,0,43q�68   $��Bottom=�6e   $��55��6�   .��6�   	Launch...6�   �� 5   
   -   5        $��btnScenario��5   L   $��23��6   $
��Left=�6   $��
83,555,0,0q�68   $	��Top=�6_   $��61��6w   .��6�   	Load Sc6�   �� 5   
   -   5        $��btnStartGame��5   K   $��23��6   $
��Left=�6   $��	83,0,0,10q�68   $��Bottom=�6_   $��61��6z   .��6�   START!6�   �� 5   
   -   5        $��btnInfo��5   L   $��23��6   $
��Left=�6   $��
150,0,0,44q�68   $��Bottom=�6[   $��58��6v   .��6�   Info6�   �� 5   
   -   5        $��chkClearWaypoints��5   O   $��20��6   $��Right=�6   $��
0,0,362,13q�6;   $��Bottom=�6h   $��42��6�    d� 	IsChecked$
 TrueY�6�    � ToolTip$ Clear old waypoints��6�   $��RightToLeft=�6�   .��6�   Clr6�   �� 5   
   -   5        $��btnDesignateHostile��5   L   $��23��6   $
��Left=�6   $��
214,0,0,10q�68   $��Bottom=�6g   $��55��6�   .��6�   Foe6�   �� 5    
   -   5        $��btnRadar��5    L   $��23��6   $
��Left=�6   $��
150,0,0,10q�68   $��Bottom=�6\   $��58��6w   .��6�   Radar6�   �� 5!   
   -   5        $��btnCloseAndEngage��5!   3   $��23��6   $��380,0,419,44q�6   $��Bottom=�6L   .��6�   CloseEngage6�   �� 5"   
   -   5        $��txtScenario��5"   2   $��23��6   $��
348,12,0,0q�6   $	��Top=�6E   $
��Left=�6]   $��70��6x   $��gamescom2011small��6�   6�   �� 5#   
   -   5        $��lblScenario��5#   0   $��28��6   $��
322,12,0,0q�6   $	��Top=�6C   $
��Left=�6[   $��30��6v   .��6�   Sc:6�   �� 5$   
   -   5        $��
cmbPlayers��5$   O   $��23��6   $��Right=�6   $��
0,12,273,0q�6;   $	��Top=�6a   $	��120��6y   �� 5%   
   -    5        $��lblScenarioPlayer��5%   2   $��28��6   $��424,12,399,0q�6   $	��Top=�6K   .��6c   Pl:6h   �� 5&   
   -!   5        $��btnRtb��5&   1   $��23��6   $��
275,0,0,43q�6   $��Bottom=�6?   $
��Left=�6Z   $��46��6u   .��6�   RTB6�   �� 5'   
   -"   5        $��btnCheat��5'   L   $��23��6   $
��Left=�6   $��
275,0,0,10q�68   $��Bottom=�6\   $��46��6w   .��6�   Cheat6�   �� 5(   
   -#   5        $��btnStartServer��5(   c   $��Start Server��6   $��23��6(   $
��Left=�64   $��
768,11,0,0q�6O   $	��Top=�6y   $��75��6�   6�   �� 5)   
   -$   5        $��btnMaxSpeed��5)   Z   $	��Max��6   $��23��6   $
��Left=�6+   $��
328,0,0,43q�6F   $��Bottom=�6m   $��46��6�   6�   5*      5+      