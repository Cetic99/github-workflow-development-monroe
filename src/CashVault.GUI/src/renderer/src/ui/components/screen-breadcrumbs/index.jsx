/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */

import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import Breadcrumbs from '@ui/components/breadcrumbs'
import { useAppVersion, useOperatorName } from '@domain/global/stores'

const Container = styled.div`
  display: flex;
  justify-content: space-between;

  & .info {
    display: flex;
    flex-direction: column;
    text-align: end;

    font-family: Poppins;
    font-weight: 700;
    font-size: 2.5rem;
    line-height: 2.5rem;
    letter-spacing: 0.003rem;

    & .message {
      font-weight: 600;
      font-size: 1rem;
      line-height: 1.125rem;
      text-transform: uppercase;
      color: var(--secondary-dark);
    }

    & .app-version {
      color: black;
      font-size: 1rem;
      text-align: right;
      position: absolute;
      top: 7.5rem;
      right: 4rem;
      font-weight: 400;
    }
  }
`

const ScreenBreadcrumbs = ({ name, names }) => {
  const operatorName = useOperatorName()
  const { t } = useTranslation()
  const appVersion = useAppVersion()

  return (
    <Container>
      <Breadcrumbs name={name} names={names} />

      {operatorName && (
        <div className="info">
          <div className="app-version">
            {t('Version')}: {appVersion}
          </div>
          <div className="message">{t('welcome')}</div>
          <div>{operatorName}</div>
        </div>
      )}
    </Container>
  )
}

export default ScreenBreadcrumbs
