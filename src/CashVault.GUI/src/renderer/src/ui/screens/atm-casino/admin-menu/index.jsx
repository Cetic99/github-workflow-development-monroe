/* eslint-disable prettier/prettier */
import { useTranslation } from '@domain/administration/stores'
import styled from '@emotion/styled'
import ScreenContainer from '@ui/components/screen-container'
import CardButton from '@ui/components/card-button'
import IconToggleOn from '@ui/components/icons/IconToggleOn'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;
  height: 100%;
  overflow: hidden;

  & .content {
    display: flex;
    flex-direction: column;
    gap: 2.5rem;
    height: 100%;
    overflow: auto;
    padding: 1.5rem 5.25rem 2rem 5.25rem;
  }

  & .menu {
    display: flex;
    flex-direction: column;
    gap: 1rem;

    & .row-2 {
      display: grid;
      grid-template-columns: 1fr 1fr;
      grid-gap: 1rem;
    }

    & .row-3 {
      display: grid;
      grid-template-columns: 1fr 1fr 1fr;
      grid-gap: 1rem;
    }
  }

  & .menu-content {
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  & .menu-header {
    font-weight: 600;
    font-size: 1.625rem;
    line-height: 1.75rem;
  }
`

const AdminMenuScreen = () => {
  const { t } = useTranslation()

  //===========================================================================

  return (
    <ScreenContainer isAdmin={false} overflow={false} padding={false} hasFadeLogo={true}>
      <Container>
        <div className="content">
          <div className="menu">
            <div className="menu-header">{t('Please choose')}</div>

            <div className="menu-content">
              <div className="row-3">
                <CardButton
                  text={t('Money service')}
                  icon={<IconToggleOn size="l" color="var(--primary-light)" />}
                />

                <CardButton
                  text={t('Reports')}
                  icon={<IconToggleOn size="l" color="var(--primary-light)" />}
                />

                <CardButton
                  text={t('Logs')}
                  icon={<IconToggleOn size="l" color="var(--primary-light)" />}
                />
              </div>

              <div className="row-2">
                <CardButton
                  text={t('Admin')}
                  icon={<IconToggleOn size="l" color="var(--primary-light)" />}
                />

                <CardButton
                  text={t('Maintenance')}
                  icon={<IconToggleOn size="l" color="var(--primary-light)" />}
                />
              </div>
            </div>
          </div>

          <div className="menu">
            <div className="menu-header">{t('Configuration')}</div>

            <div className="menu-content">
              <div className="row-3">
                <CardButton
                  text={t('Money')}
                  icon={<IconToggleOn size="l" color="var(--primary-light)" />}
                />

                <CardButton
                  text={t('Customiz.')}
                  icon={<IconToggleOn size="l" color="var(--primary-light)" />}
                />

                <CardButton
                  text={t('Devices')}
                  icon={<IconToggleOn size="l" color="var(--primary-light)" />}
                />
              </div>
            </div>
          </div>
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default AdminMenuScreen
