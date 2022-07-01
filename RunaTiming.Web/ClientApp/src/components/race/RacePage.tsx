import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { NoWrapTd, Table, TableContainer } from '../../styles/styles';
import {
  getDateString, getKmString, getPaceString, getTimeString,
} from '../utils/Formatters';
import Loader, { LoadingStatus } from '../utils/Loader';

interface RaceTime {
  durationInSeconds: number;
  distanceInMeters: number;
  positionOverall: number;
  positionClass: number;
  positionSex: number;
}

interface RaceResult {
  bib: number;
  firstName: string;
  lastName: string;
  finish?: RaceTime | null;
}

const RacePage: React.FC = () => {
  const [loadingStatus, setLoadingStatus] = useState(LoadingStatus.None);
  const [raceResults, setRaceResults] = useState<RaceResult[]>();
  const { id } = useParams<{ id: string | undefined }>();

  useEffect(() => {
    if (raceResults != null) {
      return;
    }

    setLoadingStatus(LoadingStatus.Loading);

    fetch(`/api/race/${id}/results`)
      .then((response) => response.json() as Promise<RaceResult[]>)
      .then((data) => {
        setRaceResults(data);
        setLoadingStatus(LoadingStatus.None);
      })
      .catch(() => {
        setRaceResults([]);
        setLoadingStatus(LoadingStatus.Error);
      });
  }, [raceResults]);

  let fastestDuration = 0;

  return (
    <>
      <Loader status={loadingStatus} />
      {loadingStatus === LoadingStatus.None && raceResults && (
        <TableContainer>
          <Table>
            <thead>
              <tr>
                <th>Plass</th>
                <th>Startnr</th>
                <th>Navn</th>
                <th>Tid</th>
                <th>Etter</th>
                <th>min/km</th>
              </tr>
            </thead>
            <tbody>
              {raceResults?.map((raceResult) => {
                if (fastestDuration === 0 && raceResult.finish) {
                  fastestDuration = raceResult.finish.durationInSeconds;
                }

                return (
                  <tr key={raceResult.bib}>
                    <NoWrapTd>{raceResult.finish?.positionOverall}</NoWrapTd>
                    <NoWrapTd>{raceResult.bib}</NoWrapTd>
                    <NoWrapTd>{raceResult.firstName} <span style={{ textTransform: 'uppercase' }}>{raceResult.lastName}</span></NoWrapTd>
                    {
                    raceResult.finish && (
                      <>
                        <NoWrapTd>{getTimeString(raceResult.finish.durationInSeconds)}</NoWrapTd>
                        <NoWrapTd>+{getTimeString(raceResult.finish.durationInSeconds - fastestDuration)}</NoWrapTd>
                        <NoWrapTd>{getPaceString(raceResult.finish.distanceInMeters / raceResult.finish.durationInSeconds)}</NoWrapTd>
                      </>
                    )
                  }
                  </tr>
                );
              })}
            </tbody>
          </Table>
        </TableContainer>
      )}
    </>
  );
};

export default RacePage;
