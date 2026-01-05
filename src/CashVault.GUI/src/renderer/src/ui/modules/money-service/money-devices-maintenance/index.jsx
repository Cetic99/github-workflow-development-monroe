/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import Button from '@ui/components/button'
import IconToggleOff from '@ui/components/icons/IconToggleOff'
import IconToggleOn from '@ui/components/icons/IconToggleOn'
import { useTranslation } from '@domain/administration/stores'
import CheckboxInput from '@ui/components/inputs/checkbox-input'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  background: white;
  border-radius: 10px;
  padding: 1.5rem;
  gap: 1rem;
`

const Item = styled.div`
  display: flex;
  flex-direction: column;

  ${(p) => {
    if (p.hasDivider) {
      return `
        padding-top: 1rem;
        border-top: 1px solid var(--border-light);
      `
    }
  }}

  & .name {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
  }

  & .info {
    display: flex;
    align-items: center;
  }

  & .status {
    display: flex;
    gap: 1rem;
    align-items: center;

    font-weight: 500;
    font-size: 1.5rem;
    line-height: 1.875rem;

    & .active {
      display: flex;
      align-items: center;
      gap: 0.25rem;
      font-weight: 600;
      color: var(--primary-dark);
    }

    & .enabled {
      display: flex;
      align-items: center;
      gap: 0.25rem;
    }
  }

  & .divider {
    width: 1px;
    height: 2rem;
    background-color: var(--primary-medium);
  }

  & .action-button {
    margin-left: auto;
  }
`

const MoneyDevicesMaintenance = ({ devices = [] }) => {
  const { t } = useTranslation()

  if (devices?.length > 0)
    return (
      <Container>
        {devices?.map((x, i) => (
          <Item key={`${x?.name}__${i}`} hasDivider={i > 0}>
            <div className="name">{x.name}</div>

            <div className="info">
              <div className="status">
                <div className="active">
                  <CheckboxInput value={x.active} size="s" />
                  {t('Active')}
                </div>

                <div className="divider" />

                <div className="enabled">
                  <CheckboxInput value={x.enabled} size="s" />
                  {x.enabled && t('Enabled')}
                  {!x.enabled && t('Disabled')}
                </div>
              </div>

              <Button
                color={x.enabled ? 'dark' : 'light'}
                className="action-button"
                icon={(props) => {
                  if (x.enabled) return <IconToggleOn {...props} />
                  if (!x.enabled) return <IconToggleOff {...props} />
                }}
                onClick={() => x?.onEnableDisable(x?.enabled, x?.type)}
              >
                {x.enabled && t('Enabled')}
                {!x.enabled && t('Disabled')}
              </Button>
            </div>
          </Item>
        ))}
      </Container>
    )

  return <></>
}

export default MoneyDevicesMaintenance
